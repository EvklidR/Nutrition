using FoodService.Domain.Interfaces;
using FoodService.Application.Exceptions;
using FoodService.Application.Interfaces;
using FoodService.Application.UseCases.Commands.DayResult;

namespace FoodService.Application.UseCases.CommandHandlers.DayResult
{
    public class DeleteDayResultHandler : ICommandHandler<DeleteDayResultCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        public DeleteDayResultHandler(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task Handle(DeleteDayResultCommand request, CancellationToken cancellationToken)
        {
            var dayResult = await _unitOfWork.DayResultRepository.GetByIdAsync(request.DayResultId);

            if (dayResult == null)
            {
                throw new NotFound("DayResult not found");
            }
               
            await _userService.CheckProfileBelongingAsync(request.UserId, dayResult.ProfileId);

            _unitOfWork.DayResultRepository.Delete(dayResult);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
