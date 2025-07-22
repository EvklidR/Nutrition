namespace FoodService.Application.DTOs.Product.Responses
{
    public class ProductsResponse
    {
        public List<ProductResponse>? Products { get; set; } 
        public long TotalCount { get; set; }
    }
}
