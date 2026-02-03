using Utility.Mediator;

namespace TaskManager.Application.Features.Users.Commands
{
    public record UploadProfileImageCommand(Stream ImageStream, string FileName) : IRequest<Unit>;
}
