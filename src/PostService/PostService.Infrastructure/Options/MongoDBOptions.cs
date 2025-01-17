namespace PostService.Infrastructure.Options
{
    public class MongoDBOptions
    {
        public string ConnectionURI { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
    }
}