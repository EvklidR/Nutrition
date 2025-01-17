namespace PostService.Core.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string OwnerEmail { get; set; }
        public string OwnerId { get; set; }
        public DateOnly Date {  get; set; }
        public string Text { get; set; }
        public List<string> UserLikeIds { get; set; } = [];
    }
}