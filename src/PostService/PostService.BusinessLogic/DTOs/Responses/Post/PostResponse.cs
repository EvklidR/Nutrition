namespace PostService.BusinessLogic.DTOs.Responses.Post
{
    public class PostResponse
    {
        public string Id { get; set; } = null!;
        public string? Title { get; set; }
        public string Text { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        public string OwnerEmail { get; set; } = null!;
        public List<string> KeyWords { get; set; } = [];
        public int AmountOfLikes { get; set; }
        public int AmountOfComments { get; set; }
        public bool IsLiked { get; set; }
        public bool IsOwner { get; set; }
    }
}
