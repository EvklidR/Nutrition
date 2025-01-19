namespace PostService.BusinessLogic.DTOs.Post
{
    public class CreatePostDTO
    {
        public string OwnerEmail { get; set; }
        public string OwnerId { get; set; }
        public string? Name { get; set; }
        public string Text { get; set; }
        public List<string> KeyWords { get; set; } = [];
    }
}
