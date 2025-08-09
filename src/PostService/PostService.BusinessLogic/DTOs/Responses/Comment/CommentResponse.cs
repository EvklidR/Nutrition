namespace PostService.BusinessLogic.DTOs.Responses.Comment
{
    public class CommentResponse
    {
        public string Id { get; set; } = null!;
        public string OwnerEmail { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        public string Text { get; set; } = null!;
        public int AmountOfLikes { get; set; }
        public bool IsLiked { get; set; }
        public bool IsOwner { get; set; }
        public bool IsEdited { get; set; }
    }
}
