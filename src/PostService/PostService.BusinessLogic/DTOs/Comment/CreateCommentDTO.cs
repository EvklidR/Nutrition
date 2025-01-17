namespace PostService.BusinessLogic.DTOs.Comment
{
    public class CreateCommentDTO
    {
        public string? SuperCommentId { get; set; }
        public string OwnerEmail { get; set; }
        public string OwnerId { get; set; }
        public string Text { get; set; }
    }
}
