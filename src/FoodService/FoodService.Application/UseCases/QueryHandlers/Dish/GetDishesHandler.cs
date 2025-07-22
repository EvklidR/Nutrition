using MediatR;
using FoodService.Domain.Interfaces;
using FoodService.Application.UseCases.Queries.Dish;
using AutoMapper;
using FoodService.Application.DTOs.Dish;
using FoodService.Application.DTOs.Dish.Responses;

namespace FoodService.Application.UseCases.QueryHandlers.Dish
{
    public class GetDishesHandler : IRequestHandler<GetDishesQuery, DishesResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetDishesHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<DishesResponse> Handle(GetDishesQuery request, CancellationToken cancellationToken)
        {
            var response = await _unitOfWork.DishRepository.GetAllAsync(request.UserId, request.Parameters);

            var dishesDTO = _mapper.Map<List<DishResponse>>(response.dishes);

            return new DishesResponse() 
            { 
                Dishes = dishesDTO,
                TotalCount = response.totalCount
            };
        }
    }
}
