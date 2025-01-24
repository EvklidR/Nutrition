namespace FoodService.Application.Interfaces
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(Stream fileStream, string path);
        Task<bool> DeleteImageAsync(string path);
    }
}
