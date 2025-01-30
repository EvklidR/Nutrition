using MediatR;
using FoodService.Domain.Interfaces;
using FoodService.Application.UseCases.Queries.Dish;
using AutoMapper;
using FoodService.Application.DTOs.Dish;

namespace FoodService.Application.UseCases.QueryHandlers.Dish
{
    public class GetDishesHandler : IRequestHandler<GetAllDishesQuery, IEnumerable<BriefDishDishDTO>?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetDishesHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BriefDishDishDTO>?> Handle(GetAllDishesQuery request, CancellationToken cancellationToken)
        {
            var dishes = await _unitOfWork.DishRepository.GetAllAsync(request.UserId, request.Parameters);

            var dishesDTO = _mapper.Map<IEnumerable<BriefDishDishDTO>>(dishes);

            return dishesDTO;
        }
    }
}
