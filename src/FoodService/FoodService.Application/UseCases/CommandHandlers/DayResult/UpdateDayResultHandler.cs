using AutoMapper;
using MediatR;
using FoodService.Application.Exceptions;
using FoodService.Domain.Interfaces;
using FoodService.Application.Interfaces;
using FoodService.Application.UseCases.Commands.DayResult;

namespace FoodService.Application.UseCases.CommandHandlers.DayResult
{
    public class UpdateDayResultHandler : ICommandHandler<UpdateDayResultCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService; 

        public UpdateDayResultHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task Handle(UpdateDayResultCommand request, CancellationToken cancellationToken)
        {
            var dayResult = await _unitOfWork.DayResultRepository.GetByIdAsync(request.UpdateDayResultDTO.Id);

            if (dayResult == null)
            {
                throw new NotFound("DayResult not found");
            }

            var doesProfileBelongUser = await _userService.CheckProfileBelonging(request.UserId, dayResult.ProfileId);

            if (!doesProfileBelongUser)
            {
                throw new Forbidden("You dont have access to this meal");
            }

            _mapper.Map(request.UpdateDayResultDTO, dayResult);

            _unitOfWork.DayResultRepository.Update(dayResult);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
