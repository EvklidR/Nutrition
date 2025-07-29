using AutoMapper;
using FoodService.Domain.Interfaces;
using FoodService.Application.UseCases.Commands.Product;
using FoodService.Application.Exceptions;
using FoodService.Application.Interfaces;
using FoodService.Application.DTOs.Product.Responses;

namespace FoodService.Application.UseCases.CommandHandlers.Product
{
    public class CreateProductHandler : ICommandHandler<CreateProductCommand, ProductResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public CreateProductHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<ProductResponse> Handle(
            CreateProductCommand request,
            CancellationToken cancellationToken)
        {
            await _userService.CheckUserByIdAsync(request.UserId!);

            var product = _mapper.Map<Domain.Entities.Product>(request.CreateProductDTO);

            _unitOfWork.ProductRepository.Add(product);

            await _unitOfWork.SaveChangesAsync();

            var productDTO = _mapper.Map<ProductResponse>(product);

            return productDTO;
        }
    }
}
