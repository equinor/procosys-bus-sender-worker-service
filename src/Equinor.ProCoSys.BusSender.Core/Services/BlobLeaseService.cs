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

namespace Equinor.ProCoSys.BusSenderWorker.Core.Services;

public class BlobLeaseService : IBlobLeaseService
{
    private readonly ILogger<BlobLeaseService> _logger;
    private readonly IConfiguration _configuration;
    private CancellationTokenSource? _cancellationTokenSource;

    public CancellationToken CancellationToken => _cancellationTokenSource?.Token ?? CancellationToken.None;

    public BlobLeaseService(ILogger<BlobLeaseService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<List<PlantLease>?> ClaimPlantLease()
    {
        if (!int.TryParse(_configuration["BlobLeaseExpiryTime"], out var blobLeaseExpiryTime))
        {
            _logger.LogError("Invalid BlobLeaseExpiryTime configuration value.");
            return null;
        }

        if (_cancellationTokenSource == null || _cancellationTokenSource.Token == CancellationToken.None)
        {
            // Multiplying by a factor lower than 1 to ensure that message processing for this instance is cancelled shortly before the lease expires.
            _cancellationTokenSource = new CancellationTokenSource((int)(blobLeaseExpiryTime * 1000 * 0.95));
        }

        var leaseId = Guid.NewGuid().ToString();
        var plantLeases = await GetPlantLeases(leaseId);
        if (plantLeases == null)
        {
            // Nothing to do for now.
            return null;
        }

        var plantLease = GetOldestUnprocessedPlantLeaseInfo(plantLeases);
        if (plantLease == null)
        {
            // Nothing to do for now.
            return null;
        }

        plantLeases.Where(x => x.Plant.Equals(plantLease.Plant)).ToList().ForEach(x =>
        {
            x.IsCurrent = true;
            x.LeaseExpiry = DateTime.UtcNow.AddSeconds(blobLeaseExpiryTime);
        });

        UpdatePlantLeases(plantLeases, leaseId);
        return plantLeases;
    }

    public async void ReleasePlantLease(PlantLease? plantLease, int maxRetryAttempts = 0)
    {
        if (plantLease == null)
        {
            _logger.LogWarning("Plant lease is null. Cannot release lease.");
            return;
        }

        var leaseId = Guid.NewGuid().ToString();
        var plantLeases = await GetPlantLeases(leaseId,maxRetryAttempts);
        if (plantLeases == null)
        {
            _logger.LogWarning("Plant leases is null. Cannot release lease.");
            return;
        }

        plantLeases.Where(x => x.Plant.Equals(plantLease?.Plant)).ToList().ForEach(x =>
        {
            x.LeaseExpiry = null;
            x.LastProcessed = DateTime.UtcNow;
        });
        UpdatePlantLeases(plantLeases, leaseId);
    }

    public virtual BlobLeaseClient GetBlobLeaseClient(BlobClient blobClient, string leaseId) => blobClient.GetBlobLeaseClient(leaseId);

    protected async Task<bool> TryAcquireBlobLeaseAsync(BlobClient blobClient, string leaseId, TimeSpan leaseDuration, int maxRetryAttempts = 0, TimeSpan? delayBetweenAttempts = null)
    {
        if (!int.TryParse(_configuration["BlobReleaseLeaseDelay"], out var blobReleaseLeaseDelay))
        {
            _logger.LogError("Invalid BlobReleaseLeaseDelay configuration value.");
        }
        delayBetweenAttempts ??= TimeSpan.FromSeconds(blobReleaseLeaseDelay);

        var retryPolicy = Policy
            .Handle<RequestFailedException>(ex => ex.ErrorCode == BlobErrorCode.LeaseAlreadyPresent)
            .WaitAndRetryAsync(maxRetryAttempts, retryAttempt =>
            {
                _logger.LogInformation($"Attempt {retryAttempt} to acquire lease for blob: {blobClient.Name}");
                return delayBetweenAttempts.Value;
            });

        try
        {
            await retryPolicy.ExecuteAsync(async () =>
            {
                var leaseClient = GetBlobLeaseClient(blobClient, leaseId);
                await leaseClient.AcquireAsync(leaseDuration, cancellationToken: CancellationToken.None);
            });
            return true;
        }
        catch (RequestFailedException rfe)
        {
            if (rfe.ErrorCode == BlobErrorCode.LeaseAlreadyPresent)
            {
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

    private async Task<List<PlantLease>?> GetPlantLeases(string leaseId, int maxRetryAttempts = 0)
    {
        var blobClient = GetBlobClient();
        var newLeaseAcquired = await TryAcquireBlobLeaseAsync(blobClient, leaseId, TimeSpan.FromSeconds(15), maxRetryAttempts);
        if (newLeaseAcquired)
        {
            var plantLease = await GetPlantLeases(blobClient);
            if (plantLease == null || !plantLease.Any())
            {
                _logger.LogWarning("Could not read blob containing plant lease.");
                return null;
            }

            return plantLease;
        }

        return null;
    }

    private async Task<List<PlantLease>?> GetPlantLeases(BlobClient blobClient)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new DateTimeConverter() }
        };

        var json = await GetBlobContentAsync(blobClient);
        return JsonSerializer.Deserialize<List<PlantLease>>(json, options);
    }

    public virtual async void UpdatePlantLeases(List<PlantLease> plantLeases, string leaseId)
    {
        var blobClient = GetBlobClient();
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            Converters = { new DateTimeConverter() }
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
        await blobClient.UploadAsync(memoryStream, uploadOptions, CancellationToken.None);
        await ReleaseBlobLeaseAsync(blobClient, leaseId);
    }

    public virtual BlobClient GetBlobClient()
    {
        var connectionString = _configuration["BlobStorage:ConnectionString"];
        var containerName = _configuration["BlobStorage:BusSenderContainerName"];
        var blobLeaseFileName = _configuration["BlobLeaseFileName"];
        var blobContainerClient = new BlobContainerClient(connectionString, containerName);
        return blobContainerClient.GetBlobClient(blobLeaseFileName);
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
            await leaseClient.ReleaseAsync(cancellationToken: CancellationToken);
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
}
