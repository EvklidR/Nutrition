using FoodService.Domain.Entities;

namespace FoodService.Application.Helpers;

public static class CalculationHelper
{
    public static double CalculateCaloriesOfDayResult(DayResult dayResult)
    {
        return dayResult.Meals.Select(CalculateCaloriesOfMeal).Sum();
    }

    public static double CalculateProteinsOfDayResult(DayResult dayResult)
    {
        return dayResult.Meals.Select(CalculateProteinsOfMeal).Sum();
    }

    public static double CalculateFatsOfDayResult(DayResult dayResult)
    {
        return dayResult.Meals.Select(CalculateFatsOfMeal).Sum();
    }

    public static double CalculateCarbohydratesOfDayResult(DayResult dayResult)
    {
        return dayResult.Meals.Select(CalculateCarbohydratesOfMeal).Sum();
    }

    public static double CalculateCaloriesOfMeal(Meal meal)
    {
        var caloriesOfProducts = meal.Products
            .Select(eatenProduct =>
                eatenProduct.Product.Calories * eatenProduct.Weight / 100)
            .Sum();

        var caloriesOfDishes = meal.Dishes
            .Select(eatenDish =>
                eatenDish.Dish.Calories * eatenDish.Dish.WeightOfPortion * eatenDish.AmountOfPortions / 100)
            .Sum();

        return caloriesOfDishes + caloriesOfProducts;
    }

    public static double CalculateProteinsOfMeal(Meal meal)
    {
        var proteinsOfProducts = meal.Products
            .Select(eatenProduct =>
                eatenProduct.Product.Proteins * eatenProduct.Weight / 100)
            .Sum();

        var proteinsOfDishes = meal.Dishes
            .Select(eatenDish =>
                eatenDish.Dish.Proteins * eatenDish.Dish.WeightOfPortion * eatenDish.AmountOfPortions / 100)
            .Sum();

        return proteinsOfDishes + proteinsOfProducts;
    }

    public static double CalculateFatsOfMeal(Meal meal)
    {
        var fatsOfProducts = meal.Products
            .Select(eatenProduct =>
                eatenProduct.Product.Fats * eatenProduct.Weight / 100)
            .Sum();

        var fatsOfDishes = meal.Dishes
            .Select(eatenDish =>
                eatenDish.Dish.Fats * eatenDish.Dish.WeightOfPortion * eatenDish.AmountOfPortions / 100)
            .Sum();

        return fatsOfDishes + fatsOfProducts;
    }

    public static double CalculateCarbohydratesOfMeal(Meal meal)
    {
        var carbsOfProducts = meal.Products
            .Select(eatenProduct =>
                eatenProduct.Product.Carbohydrates * eatenProduct.Weight / 100)
            .Sum();

        var carbsOfDishes = meal.Dishes
            .Select(eatenDish =>
                eatenDish.Dish.Carbohydrates * eatenDish.Dish.WeightOfPortion * eatenDish.AmountOfPortions / 100)
            .Sum();

        return carbsOfDishes + carbsOfProducts;
    }



    public class Nutrients
    {
        public double Calories { get; set; }
        public double Proteins { get; set; }
        public double Fats { get; set; }
        public double Carbohydrates { get; set; }
    }
}
