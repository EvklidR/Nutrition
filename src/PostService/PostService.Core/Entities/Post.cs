using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PostService.Core.Entities
{
    public class Post
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string? Name { get; set; }
        public string Text { get; set; }
        public List<string> KeyWords { get; set; } = [];
        public List<Comment> Comments { get; set; } = [];
        public List<string> UserLikeIds { get; set; } = [];
    }
}