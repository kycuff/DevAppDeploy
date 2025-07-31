using Microsoft.AspNetCore.Components.Forms;

namespace DevAppDeploy.Services.Blob;

public interface IBlobStorageService
{
    Task<string> UploadFileAsync(IBrowserFile file, string containerName);
    Task<Stream> DownloadFileByUrlAsync(string fileUrl);
    Task DeleteFileByUrlAsync(string fileUrl);

}
