using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Features.Users.Commands;
using TaskManager.Application.Features.Users.DTOs;
using TaskManager.Application.Features.Users.Query;
using Utility.Mediator;

namespace TaskManager.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("profile-image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            using var stream = file.OpenReadStream();
            var command = new UploadProfileImageCommand(stream, file.FileName);
            var imagePath = await _mediator.Send(command);

            return Ok(new { imagePath, message = "Profile image uploaded successfully" });
        }

        [HttpGet("profile-image")]
        public async Task<IActionResult> GetProfileImage()
        {
            ProfileImageDto profileImageDto = await _mediator.Send(new GetProfileImageQuery());

            return File(profileImageDto.FileStream, profileImageDto.ContentType, profileImageDto.FileName);
        }
    }
}
