using Dropbox.Api;
using Dropbox.Api.Files;
using FoodService.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace FoodService.Infrastructure.Services
{
    public class ImageService : IImageService
    {
        private readonly DropboxClient _dropboxClient;
        private readonly ICacheService _cacheService;

        public ImageService(string appKey, string appSecret, string refreshToken, ICacheService cacheService)
        {
            _dropboxClient = new DropboxClient(refreshToken, appKey, appSecret);
            _cacheService = cacheService;
        }

        public async Task<string?> UploadImageAsync(IFormFile? file)
        {
            var isFileValid = (file != null) && (file.Length > 0);

            if (isFileValid) 
            {
                var extention = file!.FileName.Split('.').Last();

                var dropboxPath = $"/Dishes/{Guid.NewGuid()}.{extention}";

                using (var stream = file.OpenReadStream())
                {
                    try
                    {
                        var uploadResult = await _dropboxClient.Files.UploadAsync(
                            path: dropboxPath,
                            mode: WriteMode.Add.Instance,
                            body: stream
                        );

                        return dropboxPath;
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> DeleteImageAsync(string dropboxPath)
        {
            try
            {
                await _dropboxClient.Files.DeleteV2Async(dropboxPath);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Stream?> DownloadImageAsync(string dropboxPath)
        {
            try
            {
                var response = await _cacheService.GetCachedAsync<byte[]?>(dropboxPath);

                if (response.isFound)
                {
                    return new MemoryStream(response.data!);
                }

                var downloadResponse = await _dropboxClient.Files.DownloadAsync(dropboxPath);

                var contentBytes = await downloadResponse.GetContentAsByteArrayAsync();

                await _cacheService.WriteAsync(dropboxPath, contentBytes);

                return new MemoryStream(contentBytes);
            }
            catch
            {
                return null;
            }
        }
    }
}
