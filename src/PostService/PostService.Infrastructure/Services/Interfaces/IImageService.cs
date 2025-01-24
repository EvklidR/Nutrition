namespace PostService.Infrastructure.Services.Interfaces
{
    public interface IImageService
    {
        Task<bool> UploadImageAsync(Stream fileStream, string dropboxPath);
        Task<bool> DeleteImageAsync(string dropboxPath);
        Task<Stream?> DownloadImageAsync(string dropboxPath);
    }
}
