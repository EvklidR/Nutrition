using Microsoft.AspNetCore.Http;

namespace FoodService.Application.Interfaces
{
    public interface IImageService
    {
        Task<string?> UploadImageAsync(IFormFile? file);
        Task<bool> DeleteImageAsync(string Path);
        Task<Stream?> DownloadImageAsync(string Path);
    }
}
