namespace PostService.BusinessLogic.DTOs.Post
{
    public class PostDTO
    {
        public string Id { get; set; }
        public string? Title { get; set; }
        public string Text { get; set; }
        public string OwnerEmail { get; set; }
        public List<string> KeyWords { get; set; } = [];
        public int AmountOfLikes { get; set; }
        public int AmountOfComments { get; set; }
        public bool IsLiked { get; set; }
    }
}
