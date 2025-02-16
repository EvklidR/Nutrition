using FoodService.Domain.Interfaces;
using FoodService.Application.DTOs.Product;
using AutoMapper;
using FoodService.Application.UseCases.Queries.Product;
using FoodService.Application.Models;

namespace FoodService.Application.UseCases.QueryHandlers.Product
{
    public class GetProductsHandler : IQueryHandler<GetProductsQuery, ProductsResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProductsResponse> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var response = await _unitOfWork.ProductRepository.GetAllAsync(request.UserId, request.Parameters);
            
            var productsDTO = _mapper.Map<List<ProductDTO>>(response.products);

            return new ProductsResponse()
            {
                Products = productsDTO,
                TotalCount = response.totalCount
            };
        }
    }
}
