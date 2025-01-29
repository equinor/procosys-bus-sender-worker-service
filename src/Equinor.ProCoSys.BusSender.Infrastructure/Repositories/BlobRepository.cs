using System;
using System.IO;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Repositories;

public class BlobRepository
{
    private readonly BlobContainerClient _client;

    public BlobRepository(string connectionString, string containerName)
        => _client = new BlobContainerClient(connectionString, containerName);

    public async void Download(string pathAndFileName)
    {
        var blobClient = _client.GetBlobClient(pathAndFileName);
        var response = await blobClient.DownloadAsync();

        var info = new DirectoryInfo("./Wallets");
        if (!info.Exists)
        {
            info.Create();
            Console.WriteLine("Created wallet directory at: " + info.FullName);
        }
        else
        {
            Console.WriteLine("Wallet directory already exists at: " + info.FullName);
        }

        using (var outputFileStream = new FileStream("./Wallets/cwallet.sso", FileMode.Create))
        {
            await response.Value.Content.CopyToAsync(outputFileStream);
        }
    }
}
