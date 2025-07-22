namespace FoodService.Application.DTOs.Recipe.Responses
{
    public class CalculatedRecipeResponse : RecipeResponse
    {
        public double Calories { get; set; }
        public double Proteins { get; set; }
        public double Fats { get; set; }
        public double Carbohydrates { get; set; }
    }
}
