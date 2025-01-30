using MediatR;
using FoodService.Domain.Interfaces;
using FoodService.Application.DTOs.Product;
using AutoMapper;
using FoodService.Application.UseCases.Queries.Product;

namespace FoodService.Application.UseCases.QueryHandlers.Product
{
    public class GetProductsHandler : IRequestHandler<GetProductsQuery, IEnumerable<ProductDTO>?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDTO>?> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _unitOfWork.ProductRepository.GetAllAsync(request.UserId, request.Parameters);
            
            var productsDTO = _mapper.Map<List<ProductDTO>>(products);

            return productsDTO;
        }
    }
}
