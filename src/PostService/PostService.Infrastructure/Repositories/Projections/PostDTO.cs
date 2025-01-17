namespace PostService.Infrastructure.Repositories.Projections
{
    public class PostDTO
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string Text { get; set; }
        public List<string> KeyWords { get; set; } = [];
        public int AmountOfLikes { get; set; }
        public int AmountOfComments { get; set; }
        public bool IsLiked { get; set; }
    }
}
