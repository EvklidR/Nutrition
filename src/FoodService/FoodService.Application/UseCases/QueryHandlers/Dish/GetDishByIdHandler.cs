using AutoMapper;
using FoodService.Domain.Interfaces;
using FoodService.Application.Exceptions;
using FoodService.Application.UseCases.Queries.Dish;
using FoodService.Application.DTOs.Dish;

namespace FoodService.Application.UseCases.QueryHandlers.Dish
{
    public class GetDishByIdHandler : IQueryHandler<GetDishByIdQuery, FullDishDishDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetDishByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<FullDishDishDTO> Handle(GetDishByIdQuery request, CancellationToken cancellationToken)
        {
            var dish = await _unitOfWork.DishRepository.GetByIdAsync(request.DishId);

            if (dish == null)
            {
                throw new NotFound("Dish not found");
            }

            if (dish.UserId != request.UserId)
            {
                throw new Forbidden("You dont have access to this dish");
            }

            var dishDTO = _mapper.Map<FullDishDishDTO>(dish);

            return dishDTO;
        }
    }
}
