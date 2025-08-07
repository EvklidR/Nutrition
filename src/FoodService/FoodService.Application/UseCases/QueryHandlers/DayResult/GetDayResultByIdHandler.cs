using AutoMapper;
using FoodService.Application.DTOs.DayResult.Responses;
using FoodService.Application.Interfaces;
using FoodService.Application.UseCases.Queries.DayResult;
using FoodService.Domain.Interfaces;

namespace FoodService.Application.UseCases.QueryHandlers.DayResult
{
    public class GetDayResultsByPeriodQueryHandler 
        : IQueryHandler<GetDayResultByIdQuery, DayResultResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public GetDayResultsByPeriodQueryHandler(IUnitOfWork unitOfWork, IUserService userService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<DayResultResponse> Handle(
            GetDayResultByIdQuery request,
            CancellationToken cancellationToken)
        {
            await _userService.CheckProfileBelongingAsync(
                request.UserId,
                request.ProfileId);

            var dayResult = await _unitOfWork.DayResultRepository.GetByIdAsync(request.DayResultId);

            var dayResultsDTO = _mapper.Map<DayResultResponse>(dayResult);

            return dayResultsDTO;
        }
    }
}
