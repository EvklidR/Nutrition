using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PostService.Core.Entities
{
    public class Post
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string? Title { get; set; }
        public string Text { get; set; }
        public DateOnly Date { get; set; }
        public string OwnerEmail { get; set; }
        public string OwnerId { get; set; }
        public List<string> KeyWords { get; set; } = [];
        public List<Comment> Comments { get; set; } = [];
        public List<string> UserLikeIds { get; set; } = [];
    }
}