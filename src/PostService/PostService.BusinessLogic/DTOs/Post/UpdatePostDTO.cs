namespace PostService.BusinessLogic.DTOs.Post
{
    public class UpdatePostDTO
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string Text { get; set; }
        public List<string> KeyWords { get; set; } = [];
    }
}
