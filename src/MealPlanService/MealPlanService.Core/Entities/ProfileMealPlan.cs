using MealPlanService.Core.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MealPlanService.Core.Entities
{
    public class ProfileMealPlan
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string MealPlanId { get; set; }
        public string ProfileId { get; set; }
        public bool IsActive { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
