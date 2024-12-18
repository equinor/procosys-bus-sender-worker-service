using System;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Repositories;

public class BlobRepository
{
    private readonly BlobContainerClient _client;

    public BlobRepository(string connectionString, string containerName)
        => _client = new BlobContainerClient(connectionString, containerName);

    // Azure Blob Storage's lease mechanism is used to as locking mechanism to ensure that only one web job can access a blob at a time.
    // This is important to prevent multiple web jobs accessing wallet file at the same time, during startup, leading to error message telling that
    // the related file can not be access since it is used by another process.
    public async void Download(string pathAndFileName, string downloadPath)
    {
        var blobClient = _client.GetBlobClient(pathAndFileName);
        if (await blobClient.ExistsAsync())
        {
            var leaseClient = blobClient.GetBlobLeaseClient();
            // Lease Duration: The lease duration is the maximum time the lease is held if not explicitly released.
            // This ensures that if the web job crashes or fails to release the lease, the lease will automatically expire after the specified duration,
            // allowing other web jobs to acquire the lease and proceed.
            var leaseId = await leaseClient.AcquireAsync(TimeSpan.FromSeconds(60));

            try
            {
                await blobClient.DownloadToAsync(downloadPath);
            }
            finally
            {
                // Explicit Release: Explicitly releasing the lease as soon as the download is complete ensures that other web jobs do not have to wait
                // for the full lease duration to expire before they can acquire the lease and access the blob.This improves efficiency and reduces
                // unnecessary waiting time.
                await leaseClient.ReleaseAsync();
            }
        }
    }
}
