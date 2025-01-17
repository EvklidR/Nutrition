namespace PostService.Infrastructure.Repositories.Projections
{
    public class CommentDTO
    {
        public Guid Id { get; set; }
        public string OwnerEmail { get; set; }
        public string OwnerId { get; set; }
        public DateOnly Date { get; set; }
        public string Text { get; set; }
        public int AmountOfLikes { get; set; }
        public bool IsLiked { get; set; }
    }
}
