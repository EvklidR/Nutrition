using PostService.BusinessLogic.DTOs.Post;

namespace PostService.BusinessLogic.Models
{
    public class PostsResponseModel
    {
        public List<PostDTO>? Posts { get; set; }
        public long TotalCount { get; set; }
    }
}
