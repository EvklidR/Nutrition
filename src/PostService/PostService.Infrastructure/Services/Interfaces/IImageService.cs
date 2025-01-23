namespace PostService.Infrastructure.Services.Interfaces
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(Stream fileStream, string dropboxPath);
        Task<bool> DeleteImageAsync(string dropboxPath);
    }
}
