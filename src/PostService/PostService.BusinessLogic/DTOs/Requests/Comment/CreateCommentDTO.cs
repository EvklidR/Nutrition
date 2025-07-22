namespace PostService.BusinessLogic.DTOs.Requests.Comment
{
    public class CreateCommentDTO
    {
        public string PostId { get; set; } = null!;
        public string Text { get; set; } = null!;
    }
}
