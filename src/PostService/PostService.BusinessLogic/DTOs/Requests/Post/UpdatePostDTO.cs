using Microsoft.AspNetCore.Http;

namespace PostService.BusinessLogic.DTOs.Requests.Post
{
    public class UpdatePostDTO
    {
        public string Id { get; set; }
        public string? Title { get; set; }
        public string Text { get; set; }
        public List<string> KeyWords { get; set; } = [];

        public List<IFormFile> NewFiles { get; set; } = [];
    }
}
