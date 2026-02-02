using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Models;
using TaskManager.Application.Features.Auth.Commands;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using TaskManager.Infrastructure.Interfaces;

namespace TaskManager.Infrastructure.Identity
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService, IRefreshTokenRepository refreshTokenRepository, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
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

            (string token, DateTime tokenExpiry) = _tokenService.GenerateToken(newUser);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            var refreshTokenEntity = new RefreshToken(refreshToken, newUser.Id, refreshTokenExpiry, "system");
            await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new AuthResult
            {
                Token = token,
                TokenExpiry = tokenExpiry,
                Email = newUser.Email!,
                UserId = newUser.Id,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = refreshTokenExpiry
            };
        }

        public async Task<AuthResult> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email) ?? throw new AuthenticationException("Invalid credentials");
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);

            if (!result.Succeeded)
                throw new AuthenticationException("Invalid credentials");

            (string token, DateTime tokenExpiry) = _tokenService.GenerateToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            var refreshTokenEntity = new RefreshToken(refreshToken, user.Id, refreshTokenExpiry, user.Email!);
            await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new AuthResult
            {
                Token = token,
                TokenExpiry = tokenExpiry,
                Email = user.Email!,
                UserId = user.Id,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = refreshTokenExpiry
            };
        }

        public async Task<AuthResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken)
                ?? throw new AuthenticationException("Invalid refresh token");

            if (!storedToken.IsActive)
                throw new AuthenticationException("Refresh token is expired or revoked");

            var user = await _userManager.FindByIdAsync(storedToken.UserId.ToString())
                ?? throw new AuthenticationException("User not found");

            // Revoke old token
            storedToken.Revoke(user.Email!);

            // Generate new tokens
            (string newAccessToken, DateTime tokenExpiry) = _tokenService.GenerateToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var newRefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            // Store new refresh token
            var newRefreshTokenEntity = new RefreshToken(newRefreshToken, user.Id, newRefreshTokenExpiry, user.Email!);
            await _refreshTokenRepository.AddAsync(newRefreshTokenEntity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new AuthResult
            {
                Token = newAccessToken,
                TokenExpiry = tokenExpiry,
                RefreshToken = newRefreshToken,
                Email = user.Email!,
                UserId = user.Id,
                RefreshTokenExpiry = newRefreshTokenExpiry
            };
        }

        public async Task RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken)
                ?? throw new AuthenticationException("Invalid refresh token");

            storedToken.Revoke("system");
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
