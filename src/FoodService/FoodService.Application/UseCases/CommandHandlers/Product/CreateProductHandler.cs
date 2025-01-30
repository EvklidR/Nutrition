using AutoMapper;
using MediatR;
using FoodService.Domain.Interfaces;
using FoodService.Application.UseCases.Commands.Product;
using FoodService.Application.DTOs.Product;
using FoodService.Application.Exceptions;
using FoodService.Application.Interfaces;

namespace FoodService.Application.UseCases.CommandHandlers.Product
{
    public class CreateProductHandler : ICommandHandler<CreateProductCommand, ProductDTO>
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

        public async Task<ProductDTO> Handle(
            CreateProductCommand request,
            CancellationToken cancellationToken)
        {
            var doesUserExist = await _userService.CheckUserByIdAsync(
                request.CreateProductDTO.UserId);

            if (!doesUserExist)
            {
                throw new Forbidden("This user doesn't exist");
            }

            var product = _mapper.Map<Domain.Entities.Product>(request.CreateProductDTO);

            _unitOfWork.ProductRepository.Add(product);

            await _unitOfWork.SaveChangesAsync();

            var productDTO = _mapper.Map<ProductDTO>(product);

            return productDTO;
        }
    }
}
