namespace DevAppDeploy.Services.Blob;

using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Components.Forms;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;

    public BlobStorageService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task<string> UploadFileAsync(IBrowserFile file, string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();

        var blobClient = containerClient.GetBlobClient($"{Guid.NewGuid()}/{file.Name}");

        using var stream = file.OpenReadStream(50 * 1024 * 1024);
        await blobClient.UploadAsync(stream, overwrite: true);

        return blobClient.Uri.ToString();
    }

    public async Task<Stream> DownloadFileByUrlAsync(string fileUrl)
    {
        // Parse the URL to extract container and blob name
        var uri = new Uri(fileUrl);
        var segments = uri.AbsolutePath.TrimStart('/').Split('/', 2);
        if (segments.Length < 2)
            throw new ArgumentException("Invalid blob URL format.");

        string containerName = segments[0];
        string blobName = segments[1];

        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        // Generate a SAS token (read-only, valid for 5 minutes)
        if (!blobClient.CanGenerateSasUri)
            throw new InvalidOperationException("BlobClient cannot generate SAS URI. Ensure the client uses a key credential.");

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            BlobName = blobName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(5)
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        Uri sasUri = blobClient.GenerateSasUri(sasBuilder);

        // Download using the SAS URL
        var sasBlobClient = new BlobClient(sasUri);
        var response = await sasBlobClient.DownloadAsync();
        var memoryStream = new MemoryStream();
        await response.Value.Content.CopyToAsync(memoryStream);
        memoryStream.Position = 0;
        return memoryStream;
    }

    public async Task DeleteFileByUrlAsync(string fileUrl)
    {
        // Parse the URL to extract container and blob name
        var uri = new Uri(fileUrl);
        var segments = uri.AbsolutePath.TrimStart('/').Split('/', 2);
        if (segments.Length < 2)
            throw new ArgumentException("Invalid blob URL format.");

        string containerName = segments[0];
        string blobName = segments[1];

        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        await blobClient.DeleteIfExistsAsync();
    }

}

