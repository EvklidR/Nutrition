using Dropbox.Api;
using Dropbox.Api.Files;
using PostService.Infrastructure.Services.Interfaces;

namespace PostService.Infrastructure.Services
{
    public class ImageService : IImageService
    {
        private readonly DropboxClient _dropboxClient;

        public ImageService(string appKey, string appSecret, string refreshToken)
        {
            _dropboxClient = new DropboxClient(refreshToken, appKey, appSecret);
        }

        public async Task<string> UploadImageAsync(Stream fileStream, string dropboxPath)
        {
            var uploadResult = await _dropboxClient.Files.UploadAsync(
                path: dropboxPath,
                mode: WriteMode.Add.Instance,
                body: fileStream
            );

            var sharedLink = await _dropboxClient.Sharing.CreateSharedLinkWithSettingsAsync(uploadResult.PathLower);
            return sharedLink.Url.Replace("&dl=0", "&raw=1");
        }

        public async Task<bool> DeleteImageAsync(string dropboxPath)
        {
            try
            {
                await _dropboxClient.Files.DeleteV2Async(dropboxPath);
                return true;
            }
            catch (Exception ex) 
            {
                return false;
            }
        }
    }
}
