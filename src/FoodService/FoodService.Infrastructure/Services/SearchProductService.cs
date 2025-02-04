using Newtonsoft.Json;
using FoodService.Application.Models;
using FoodService.Application.Interfaces;

namespace FoodService.Infrastructure.Services
{
    public class SearchProductService : ISearchProductService
    {
        private const string ApiUrlSearch = "https://world.openfoodfacts.org/cgi/search.pl?search_terms={0}&json=true&lang=en";

        public async Task<List<ProductResponseFromAPI>?> GetProductsByName(string productName)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = string.Format(ApiUrlSearch, productName);
                var response = await client.GetStringAsync(url);
                var searchResults = JsonConvert.DeserializeObject<SearchResponse>(response);

                List<ProductResponseFromAPI> products = new List<ProductResponseFromAPI>();

                if (searchResults != null && searchResults.Products != null)
                {
                    foreach (var product in searchResults.Products)
                    {
                        products.Add(new ProductResponseFromAPI() {
                            Name = product.Name,
                            Calories = product.Nutrition?.Calories ?? 0,
                            Proteins = product.Nutrition?.Protein ?? 0,
                            Fats = product.Nutrition?.Fat ?? 0,
                            Carbohydrates = product.Nutrition?.Carbs ?? 0
                        });
                    }
                }

                return products;
            }
        }
    }
}
