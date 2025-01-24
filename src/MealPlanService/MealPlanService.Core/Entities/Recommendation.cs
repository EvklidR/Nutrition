using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MealPlanService.Core.Entities
{
    public class Recommendation
    {
        public string Text { get; set; }
    }
}
