using Newtonsoft.Json;
using System.Collections.Generic;

namespace FoodService.Infrastructure.Services
{
    // Модель для десериализации ответа от поиска
    public class SearchResponse
    {
        [JsonProperty("products")]
        public List<SearchProduct> Products { get; set; }
    }
}
