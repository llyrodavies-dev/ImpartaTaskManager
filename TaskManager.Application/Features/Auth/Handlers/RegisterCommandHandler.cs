using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Models;
using TaskManager.Application.Features.Auth.Commands;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using Utility.Mediator;

namespace TaskManager.Application.Features.Auth.Handlers
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResult>
    {
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterCommandHandler(IAuthService authService, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _authService = authService;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var authResult = await _authService.RegisterAsync(request, cancellationToken);

            User newUser = new(authResult.UserId, request.Email, request.DisplayName, "system");

            await _userRepository.AddUserAsync(newUser, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return authResult;
        }
    }
}
