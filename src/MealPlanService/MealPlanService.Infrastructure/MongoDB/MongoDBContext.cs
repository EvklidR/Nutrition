using MealPlanService.Core.Entities;
using MealPlanService.Infrastructure.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MealPlanService.Infrastructure.MongoDB
{
    public class MongoDBContext
    {
        private readonly IMongoDatabase _database;

        public MongoDBContext(IOptions<MongoDBOptions> mongoSettings)
        {
            var settings = mongoSettings.Value;
            var client = new MongoClient(settings.ConnectionURI);
            _database = client.GetDatabase(settings.DatabaseName);
        }

        public IMongoCollection<ProfileMealPlan> ProfileMealPlans => _database.GetCollection<ProfileMealPlan>("ProfileMealPlans");
        public IMongoCollection<MealPlan> MealPlans => _database.GetCollection<MealPlan>("MealPlans");
    }
}
