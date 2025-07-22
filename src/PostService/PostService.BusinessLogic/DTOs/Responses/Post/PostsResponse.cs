namespace PostService.BusinessLogic.DTOs.Responses.Post
{
    public class PostsResponse
    {
        public List<PostResponse> Posts { get; set; } = [];
        public long TotalCount { get; set; }
    }
}
