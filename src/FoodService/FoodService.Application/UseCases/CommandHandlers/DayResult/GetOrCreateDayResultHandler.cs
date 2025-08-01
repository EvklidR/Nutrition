using FoodService.Domain.Interfaces;
using FoodService.Application.Interfaces;
using AutoMapper;
using FoodService.Application.UseCases.Commands.DayResult;
using FoodService.Application.DTOs.DayResult.Responses;

namespace FoodService.Application.UseCases.CommandHandlers.DayResult
{
    public class GetOrCreateDayResultHandler 
        : ICommandHandler<GetOrCreateDayResultCommand, DayResultResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public GetOrCreateDayResultHandler(IUnitOfWork unitOfWork, IUserService userService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<DayResultResponse> Handle(
            GetOrCreateDayResultCommand request,
            CancellationToken cancellationToken)
        {
            var profileWeight = await _userService.GetProfileWeightAsync(request.ProfileId);

            var currentDay = DateOnly.FromDateTime(DateTime.Now);

            var dayResult = await _unitOfWork.DayResultRepository.GetByDateAsync(request.ProfileId, currentDay);

            if (dayResult == null)
            {
                dayResult = new Domain.Entities.DayResult
                {
                    ProfileId = request.ProfileId,
                    Date = currentDay,
                    GlassesOfWater = 0,
                    Weight = profileWeight
                };

                _unitOfWork.DayResultRepository.Add(dayResult);

                await _unitOfWork.SaveChangesAsync();
            }

            var dayResultDTO = _mapper.Map<DayResultResponse>(dayResult);

            return dayResultDTO;
        }
    }
}