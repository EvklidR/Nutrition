namespace PostService.Core.Entities
{
    public class Comment
    {
        public string Text { get; set; }
        public List<string> UserLikeIds { get; set; } = [];
        public List<Comment> SubComments { get; set; } = [];
    }
}