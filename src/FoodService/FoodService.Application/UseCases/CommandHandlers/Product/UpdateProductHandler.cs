using AutoMapper;
using FoodService.Application.Exceptions;
using FoodService.Domain.Interfaces;
using FoodService.Application.UseCases.Commands.Product;

namespace FoodService.Application.UseCases.CommandHandlers.Product
{
    public class UpdateProductHandler : ICommandHandler<UpdateProductCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateProductHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(request.UpdateProductDTO.Id);

            if (product == null)
            {
                throw new NotFound("Ingredient not found");
            }

            if (product.UserId != request.UserId)
            {
                throw new Forbidden("You dont have access to this product");
            }

            _mapper.Map(request.UpdateProductDTO, product);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
