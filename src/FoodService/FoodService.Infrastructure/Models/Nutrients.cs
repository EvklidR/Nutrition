using Newtonsoft.Json;

namespace FoodService.Infrastructure.Services
{
    public class Nutrients
    {
        [JsonProperty("energy-kcal_100g")]
        public double? Calories { get; set; }

        [JsonProperty("fat_100g")]
        public double? Fat { get; set; }

        [JsonProperty("proteins_100g")]
        public double? Protein { get; set; }

        [JsonProperty("carbohydrates_100g")]
        public double? Carbs { get; set; }
    }
}
