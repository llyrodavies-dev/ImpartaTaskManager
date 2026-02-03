namespace TaskManager.Application.Features.Users.DTOs
{
    public record ProfileImageDto(Stream FileStream, string FileName, string ContentType);
}
