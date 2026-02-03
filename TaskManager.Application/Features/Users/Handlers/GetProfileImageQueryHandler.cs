using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Features.Users.DTOs;
using TaskManager.Application.Features.Users.Query;
using Utility.Mediator;

namespace TaskManager.Application.Features.Users.Handlers
{
    public class GetProfileImageQueryHandler : IRequestHandler<GetProfileImageQuery, ProfileImageDto>
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IUserAuthorizationService _userAuthorizationService;

        public GetProfileImageQueryHandler(IFileStorageService fileStorageService, IUserAuthorizationService userAuthorizationService)
        {
            _fileStorageService = fileStorageService;
            _userAuthorizationService = userAuthorizationService;
        }

        public async Task<ProfileImageDto> Handle(GetProfileImageQuery request, CancellationToken cancellationToken = default)
        {
            var user = await _userAuthorizationService.GetAuthenticatedUserAsync(cancellationToken);

            if (string.IsNullOrEmpty(user.ProfileImagePath))
            {
                // Return default profile image
                var (defaultFileStream, defaultContentType) = await _fileStorageService.GetDefaultProfileImageAsync(cancellationToken);
                return new ProfileImageDto(defaultFileStream, "default-profile.png", defaultContentType);
            }

            var (fileStream, contentType) = await _fileStorageService.GetFileAsync(user.ProfileImagePath, cancellationToken);
            var fileName = Path.GetFileName(user.ProfileImagePath);

            return new ProfileImageDto(fileStream, fileName, contentType);
        }
    }
}
