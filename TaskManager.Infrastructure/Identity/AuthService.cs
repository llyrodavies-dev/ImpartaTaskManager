using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Models;
using TaskManager.Application.Features.Auth.Commands;
using TaskManager.Infrastructure.Interfaces;

namespace TaskManager.Infrastructure.Identity
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<bool> UserExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user is not null;
        }

        public async Task<AuthResult> RegisterAsync(RegisterCommand request, CancellationToken cancellationToken = default)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);

            if (existingUser is not null)
                throw new ValidationException("User with this email already exists");

            var newUser = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
            };

            var result = await _userManager.CreateAsync(newUser, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ValidationException(errors);
            }

            await _userManager.AddToRoleAsync(newUser, "User");

            var token = _tokenService.GenerateToken(newUser);

            return new AuthResult
            {
                Token = token,
                Email = newUser.Email!,
                UserId = newUser.Id
            };
        }

        public async Task<AuthResult> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email) ?? throw new AuthenticationException("Invalid credentials");
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);

            if (!result.Succeeded)
                throw new AuthenticationException("Invalid credentials");

            var token = _tokenService.GenerateToken(user);

            return new AuthResult
            {
                Token = token,
                Email = user.Email!,
                UserId = user.Id
            };
        }
    }
}
