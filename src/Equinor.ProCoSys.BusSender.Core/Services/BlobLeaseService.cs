using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Azure;
using Azure.Storage.Blobs.Specialized;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Polly;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Services;

public class BlobLeaseService : IBlobLeaseService
{
    private readonly ILogger<BlobLeaseService> _logger;
    private readonly IConfiguration _configuration;
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly IMemoryCache _cache;
    private readonly BlobClient _blobClient;
    private static readonly TimeSpan blobLeaseDuration = TimeSpan.FromSeconds(15);

    public CancellationToken CancellationToken => _cancellationTokenSource?.Token ?? CancellationToken.None;

    public BlobLeaseService(ILogger<BlobLeaseService> logger, IConfiguration configuration, IMemoryCache cache)
    {
        _logger = logger;
        _configuration = configuration;
        _cache = cache;
        var sw = new Stopwatch();
        sw.Start();
        _blobClient = GetBlobClient();
    }
    public virtual IMemoryCache GetCache() => _cache;

    public virtual async Task<List<PlantLease>?> ClaimPlantLease()
    {
        if (!int.TryParse(_configuration["PlantLeaseExpiryTime"], out var plantLeaseExpiryTime))
        {
            _logger.LogError("Invalid PlantLeaseExpiryTime configuration value.");
            return null;
        }
        if (!GetCache().TryGetValue("CurrentPlantLeases", out List<PlantLease>? plantLeases))
        {
            var sw = new Stopwatch();
            sw.Start();

            plantLeases = await GetPlantLeases();
            if (plantLeases == null)
            {
                _logger.LogDebug("No blob lease available. Awaiting next loop.");
                return null;
            }

            var plantLease = GetOldestUnprocessedPlantLeaseInfo(plantLeases);
            if (plantLease == null)
            {
                // Nothing to do for now.
                _logger.LogDebug("No available plants to lease. Awaiting next loop.");
                return null;
            }

            var leaseId = Guid.NewGuid().ToString();

            plantLeases.Where(x => x.Plant.Equals(plantLease.Plant)).ToList().ForEach(x =>
            {
                x.IsCurrent = true;
                x.LeaseExpiry = DateTime.UtcNow.AddSeconds(plantLeaseExpiryTime);
            });

            var didUpdatePlantLeases = await UpdatePlantLeases(plantLeases, leaseId, 0);
            if (!didUpdatePlantLeases)
            {
                return null;
            }
            GetCache().Set("CurrentPlantLeases", plantLeases, TimeSpan.FromSeconds(plantLeaseExpiryTime * 0.95));
            _logger.LogDebug($"Claim used {sw.ElapsedMilliseconds}");
        }
        else
        {
            plantLeaseExpiryTime = GetSecondsUntilLeaseExpiry(plantLeases?.First(x => x.IsCurrent).LeaseExpiry);
            _logger.LogDebug($"Plant leases retrieved from cache. {plantLeaseExpiryTime} until expired.");
        }

        if ((_cancellationTokenSource == null || _cancellationTokenSource.Token == CancellationToken.None) && (plantLeaseExpiryTime > 0))
        {
            // Multiplying by a factor lower than 1 to ensure that message processing for this instance is cancelled shortly before the lease actually expires.
            _cancellationTokenSource = new CancellationTokenSource((int)(plantLeaseExpiryTime * 1000 * 0.95));
            _logger.LogDebug($"A new cancellation token is initialized. {plantLeaseExpiryTime} until expired.");
        }

        return plantLeases;
    }

    public async Task<bool> ReleasePlantLease(PlantLease? plantLease)
    {
        if (plantLease == null)
        {
            _logger.LogWarning("Cannot release lease.");
            return false;
        }

        if (!int.TryParse(_configuration["MaxBlobReleaseLeaseAttempts"], out var maxRetryAttempts))
        {
            _logger.LogError("Invalid MaxBlobReleaseLeaseAttempts configuration value.");
            return false;
        }

        var leaseId = Guid.NewGuid().ToString();
        var plantLeases = await GetPlantLeases();
        if (plantLeases == null)
        {
            _logger.LogWarning("Cannot release plant lease.");
            return false;
        }

        plantLeases.Where(x => x.Plant.Equals(plantLease?.Plant)).ToList().ForEach(x =>
        {
            x.LeaseExpiry = null;
            x.LastProcessed = DateTime.UtcNow;
        });
        var didReleasePlantLeases = await UpdatePlantLeases(plantLeases, leaseId, maxRetryAttempts);
        if (!didReleasePlantLeases)
        {
            _logger.LogWarning("Failed to update plant lease blob. Hence plant will not be handled until expired.");
        }
        GetCache().Remove("CurrentPlantLeases");
        _cancellationTokenSource?.TryReset();
        return didReleasePlantLeases;
    }

    public virtual BlobLeaseClient GetBlobLeaseClient(BlobClient blobClient, string leaseId) => blobClient.GetBlobLeaseClient(leaseId);

    private TimeSpan GetJitter(TimeSpan delayBetweenAttempts) => TimeSpan.FromMilliseconds(new Random().Next(0, (int)(delayBetweenAttempts.TotalMilliseconds * 0.25))); // 25% jitter

    protected async Task<bool> TryAcquireBlobLeaseAsync(BlobClient blobClient, string leaseId, TimeSpan leaseDuration, int maxRetryAttempts = 0, TimeSpan? delayBetweenAttempts = null)
    {
        if (!int.TryParse(_configuration["BlobReleaseLeaseDelay"], out var blobReleaseLeaseDelay))
        {
            _logger.LogError("Invalid BlobReleaseLeaseDelay configuration value.");
        }
        delayBetweenAttempts ??= TimeSpan.FromMilliseconds(blobReleaseLeaseDelay);
        var leaseClient = GetBlobLeaseClient(blobClient, leaseId);

        var retryPolicy = Policy
            .Handle<RequestFailedException>(ex => ex.ErrorCode == BlobErrorCode.LeaseAlreadyPresent)
            .WaitAndRetryAsync(maxRetryAttempts, retryAttempt =>
            {
                _logger.LogDebug($"Attempt {retryAttempt} to acquire lease for blob: {blobClient.Name}");
                var jitter = GetJitter(delayBetweenAttempts.Value);
                return delayBetweenAttempts.Value + jitter;
            });

        try
        {
            await retryPolicy.ExecuteAsync(async () =>
            {
                // GetPropertiesAsync is called upfront of AcquireAsync in an attempt to reduce the number of calls to the latter method.
                // In general, GetPropertiesAsync is expected to be quicker than AcquireAsync because it is a read-only operation,
                // whereas AcquireAsync involves state changes and additional checks.
                var properties = await blobClient.GetPropertiesAsync();
                var leaseStateUnlocked = properties.Value.LeaseStatus == LeaseStatus.Unlocked;
                var leaseStateAvailableOrExpired = properties.Value.LeaseState == LeaseState.Available || properties.Value.LeaseState == LeaseState.Expired;
                if (leaseStateUnlocked && leaseStateAvailableOrExpired)
                {
                    await leaseClient.AcquireAsync(leaseDuration, cancellationToken: CancellationToken.None);
                }
                else
                {
                    throw new RequestFailedException(409, "Lease already present", BlobErrorCode.LeaseAlreadyPresent.ToString(),
                        new InvalidOperationException("The lease is already present and cannot be acquired."));
                }
            });
            return true;
        }
        catch (RequestFailedException rfe)
        {
            if (rfe.ErrorCode == BlobErrorCode.LeaseAlreadyPresent)
            {
                // We are only interested in warning when this method has been called with acceptance for multiple retry attempts (e.g. when releasing plant lease)
                // indicating higher importance of acquiring the lease. 
                // We do not want to spam the logs with warnings when successful blob lease is of lower importance. (e.g. for Claim)
                if (maxRetryAttempts > 0)
                {
                    _logger.LogWarning(rfe,
                        $"Failed to acquire lease for blob: {blobClient.Name} after {maxRetryAttempts} attempts. ErrorCode: {rfe.ErrorCode} Message: {rfe.Message}");
                }
                return false;
            }

            _logger.LogError(rfe, $"Failed to acquire lease for blob: {blobClient.Name} ErrorCode: {rfe.ErrorCode} Message: {rfe.Message}");
            throw;
        }
    }

    public virtual async Task<string> GetBlobContentAsync(BlobClient blobClient)
    {
        try
        {
            var response = await blobClient.DownloadStreamingAsync(cancellationToken: CancellationToken.None);
            using var streamReader = new StreamReader(response.Value.Content);
            return await streamReader.ReadToEndAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download blob content: {BlobName}", blobClient.Name);
            throw;
        }
    }
    private async Task<List<PlantLease>?> GetPlantLeases()
    {
        var plantLease = await GetPlantLeases(_blobClient);
        if (plantLease!=null && plantLease.Any())
        {
            return plantLease;
        }

        _logger.LogWarning("Could not read blob containing plant lease.");
        return null;
    }

    private async Task<List<PlantLease>?> GetPlantLeases(BlobClient blobClient)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var json = await GetBlobContentAsync(blobClient);
        return JsonSerializer.Deserialize<List<PlantLease>>(json, options);
    }

    public virtual async Task<bool> UpdatePlantLeases(List<PlantLease> plantLeases, string leaseId, int maxRetryAttempts = 0)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        var json = JsonSerializer.Serialize(plantLeases, options);
        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        var uploadOptions = new BlobUploadOptions
        {
            Conditions = new BlobRequestConditions
            {
                LeaseId = leaseId
            }
        };
        var newLeaseAcquired = await TryAcquireBlobLeaseAsync(_blobClient, leaseId, blobLeaseDuration, maxRetryAttempts);
        if (!newLeaseAcquired)
        {
            return false;
        }
        await _blobClient.UploadAsync(memoryStream, uploadOptions, CancellationToken.None);
        await ReleaseBlobLeaseAsync(_blobClient, leaseId);
        return true;
    }

    public virtual BlobClient GetBlobClient()
    {
        var connectionString = _configuration["BlobStorage:ConnectionString"];
        var containerName = _configuration["BlobStorage:BusSenderContainerName"];
        var plantLeaseFileName = _configuration["PlantLeaseFileName"];
        var blobContainerClient = new BlobContainerClient(connectionString, containerName);
        return blobContainerClient.GetBlobClient(plantLeaseFileName);
    }

    private PlantLease? GetOldestUnprocessedPlantLeaseInfo(List<PlantLease> plantLeases)
    {
        var unprocessedLeaseInfos = plantLeases
            .Where(p => p.LeaseExpiry == null || p.LeaseExpiry < DateTime.UtcNow)
            .OrderBy(p => p.LastProcessed)
            .ToList();

        return unprocessedLeaseInfos.FirstOrDefault(); // Return first if none is taken.
    }
    public virtual async Task ReleaseBlobLeaseAsync(BlobClient blobClient, string leaseId)
    {
        try
        {
            var leaseClient = GetBlobLeaseClient(blobClient, leaseId);
            await leaseClient.ReleaseAsync(cancellationToken: CancellationToken.None);
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Failed to release lease for blob: {BlobName}", blobClient.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while releasing lease for blob: {BlobName}", blobClient.Name);
        }
    }
    public static int GetSecondsUntilLeaseExpiry(DateTime? leaseExpiry)
    {
        if (leaseExpiry == null)
        {
            return 0;
        }

        var timeSpan = leaseExpiry.Value - DateTime.UtcNow;
        return (int)(timeSpan.TotalSeconds > 0 ? timeSpan.TotalSeconds : 0);
    }
}
