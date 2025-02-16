using FoodService.Application.DTOs.Product;

namespace FoodService.Application.Models
{
    public class ProductsResponse
    {
        public List<ProductDTO>? Products { get; set; } 
        public long TotalCount { get; set; }
    }
}
