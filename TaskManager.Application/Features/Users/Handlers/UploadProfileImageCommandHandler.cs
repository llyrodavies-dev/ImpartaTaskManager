using Microsoft.Extensions.Options;
using TaskManager.Application.Common.Configuration;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Exceptions;
using TaskManager.Application.Features.Users.Commands;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using Utility.Mediator;

namespace TaskManager.Application.Features.Users.Handlers
{
    public class UploadProfileImageCommandHandler : IRequestHandler<UploadProfileImageCommand, Unit>
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IUserAuthorizationService _userAuthorizationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly FileStorageOptions _fileStorageOptions;

        public UploadProfileImageCommandHandler(
            IFileStorageService fileStorageService,
            IUserAuthorizationService userAuthorizationService,
            IUnitOfWork unitOfWork,
            IOptions<FileStorageOptions> fileStorageOptions)
        {
            _fileStorageService = fileStorageService;
            _userAuthorizationService = userAuthorizationService;
            _unitOfWork = unitOfWork;
            _fileStorageOptions = fileStorageOptions.Value;
        }

        public async Task<Unit> Handle(UploadProfileImageCommand request, CancellationToken cancellationToken)
        {
            User user = await _userAuthorizationService.GetAuthenticatedUserAsync(cancellationToken);

            VerifyFileImageStreamAndFileExtension(request.ImageStream, request.FileName);

            // Delete old profile image if exists
            if (!string.IsNullOrEmpty(user.ProfileImagePath))
            {
                await _fileStorageService.DeleteFileAsync(user.ProfileImagePath, cancellationToken);
            }

            var imagePath = await _fileStorageService.SaveFileAsync(request.ImageStream, request.FileName, "profile-images", cancellationToken);

            user.UpdateProfileImage(imagePath, user.Email);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        private void VerifyFileImageStreamAndFileExtension(Stream imageStream, string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (!_fileStorageOptions.AllowedImageExtensions.Contains(extension))
            {
                throw new BadRequestException($"Invalid file type. Allowed types: {string.Join(", ", _fileStorageOptions.AllowedImageExtensions)}");
            }
            if (imageStream.Length > _fileStorageOptions.MaxFileSizeBytes)
            {
                throw new BadRequestException($"File size exceeds maximum allowed size of {_fileStorageOptions.MaxFileSizeBytes / 1024 / 1024}MB");
            }
        }
    }
}
