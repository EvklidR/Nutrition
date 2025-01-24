using Dropbox.Api;
using Dropbox.Api.Files;
using PostService.Infrastructure.Services.Interfaces;

namespace PostService.Infrastructure.Services
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

        public async Task<bool> UploadImageAsync(Stream fileStream, string dropboxPath)
        {
            try
            {
                var uploadResult = await _dropboxClient.Files.UploadAsync(
                    path: dropboxPath,
                    mode: WriteMode.Add.Instance,
                    body: fileStream
                );

                return true;
            }
            catch 
            { 
                return false;
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
                var cachedResponse = await _cacheService.GetCachedImageAsync(dropboxPath);

                if (cachedResponse != null)
                {
                    return cachedResponse;
                }

                var downloadResponse = await _dropboxClient.Files.DownloadAsync(dropboxPath);

                var contentBytes = await downloadResponse.GetContentAsByteArrayAsync();

                await _cacheService.WriteImageAsync(dropboxPath, contentBytes);

                return new MemoryStream(contentBytes);
            }
            catch
            {
                return null;
            }
        }
    }
}
