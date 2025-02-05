using FoodService.Application.Exceptions;
using FoodService.Domain.Interfaces;
using FoodService.Application.Interfaces;
using FoodService.Application.DTOs.DayResult;
using AutoMapper;
using FoodService.Application.UseCases.Commands.DayResult;

namespace FoodService.Application.UseCases.CommandHandlers.DayResult
{
    public class GetOrCreateDayResultHandler 
        : ICommandHandler<GetOrCreateDayResultCommand, DayResultDTO>
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

        public async Task<DayResultDTO> Handle(
            GetOrCreateDayResultCommand request,
            CancellationToken cancellationToken)
        {
            var doesProfileBelongUser = await _userService.CheckProfileBelonging(
                request.UserId,
                request.ProfileId);

            if (!doesProfileBelongUser)
            {
                throw new Forbidden("You dont have access to this meal");
            }

            var currentDay = DateOnly.FromDateTime(DateTime.Now);

            var dayResult = await _unitOfWork.DayResultRepository.GetByDateAsync(request.ProfileId, currentDay);

            if (dayResult == null)
            {
                dayResult = new Domain.Entities.DayResult
                {
                    ProfileId = request.ProfileId,
                    Date = currentDay,
                    GlassesOfWater = 0
                };

                _unitOfWork.DayResultRepository.Add(dayResult);

                await _unitOfWork.SaveChangesAsync();
            }

            var dayResultDTO = _mapper.Map<DayResultDTO>(dayResult);

            return dayResultDTO;
        }
    }
}