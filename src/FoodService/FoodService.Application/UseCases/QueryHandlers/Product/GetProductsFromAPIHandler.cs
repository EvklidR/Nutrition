using MediatR;
using FoodService.Application.Interfaces;
using FoodService.Application.Models;
using FoodService.Application.UseCases.Queries.Product;

namespace FoodService.Application.UseCases.QueryHandlers.Product
{
    public class GetProductsFromAPIHandler : IQueryHandler<GetProductsFromAPIQuery, List<ProductResponseFromAPI>?>
    {
        private readonly ISearchProductService _searchProductService;
        public GetProductsFromAPIHandler(ISearchProductService searchProductService)
        {
            _searchProductService = searchProductService;
        }

        public async Task<List<ProductResponseFromAPI>?> Handle(GetProductsFromAPIQuery request, CancellationToken cancellationToken)
        {
            return await _searchProductService.GetProductsByName(request.Name);
        }
    }
}
