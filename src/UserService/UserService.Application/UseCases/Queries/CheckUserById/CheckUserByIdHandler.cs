using UserService.Domain.Interfaces;

namespace UserService.Application.UseCases.Queries
{
    public class CheckUserByIdHandler : IQueryHandler<CheckUserByIdQuery, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckUserByIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(CheckUserByIdQuery request, CancellationToken cancellationToken)
        {
            var existUser = await _unitOfWork.UserRepository.GetByIdAsync(request.userId);
            return existUser != null;
        }
    }
}
