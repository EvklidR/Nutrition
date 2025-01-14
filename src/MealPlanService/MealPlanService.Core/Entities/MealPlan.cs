using MealPlanService.Core.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MealPlanService.Core.Entities
{
    public class MealPlan
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public MealPlanType Type { get; set; }

        public List<MealPlanDay> Days { get; set; }
    }
}
