using Microsoft.AspNetCore.Http;

namespace PostService.BusinessLogic.DTOs.Requests.Post
{
    public class CreatePostDTO
    {
        public string? Title { get; set; }
        public string Text { get; set; } = null!;
        public List<string> KeyWords { get; set; } = [];

        public List<IFormFile> Files { get; set; } = [];
    }
}