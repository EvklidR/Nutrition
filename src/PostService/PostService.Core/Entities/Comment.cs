namespace PostService.Core.Entities
{
    public class Comment
    {
        public string Id { get; set; }
        public string OwnerEmail { get; set; }
        public string OwnerId { get; set; }
        public DateTime CreationDate {  get; set; }
        public string Text { get; set; }
        public List<string> UserLikeIds { get; set; } = [];
    }
}