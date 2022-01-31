using Azure.Storage.Blobs;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Repositories
{
    public class BlobRepository
    {
        private BlobContainerClient _client;

        public BlobRepository(string connectionString, string containerName)
        {
            _client = new BlobContainerClient(connectionString, containerName);
        }

        public async void Download(string pathAndFileName, string downloadPath)
        {
            BlobClient blobClient = _client.GetBlobClient(pathAndFileName);
            if (await blobClient.ExistsAsync())
            {
                await blobClient.DownloadToAsync(downloadPath);
            }
        }
    }
}
