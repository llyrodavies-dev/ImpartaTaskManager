using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Contracts;
using TaskManager.Application.Features.Jobs.Commands;
using TaskManager.Application.Features.Jobs.Query;
using TaskManager.Application.Features.Tasks.Commands;
using TaskManager.Application.Features.Tasks.DTOs;
using Utility.Mediator;

namespace TaskManager.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public JobsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetJobs()
        {
            return Ok(await _mediator.Send(new JobsQuery()));
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetJobById(Guid id)
        {
            return Ok(await _mediator.Send(new JobDetailsQuery(id)));
        }

        [HttpPost]
        public async Task<IActionResult> CreateJob([FromBody] CreateJobCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPost]
        [Route("{id:guid}/tasks")]
        public async Task<IActionResult> AddTaskToJob(Guid id, [FromBody] CreateTaskRequest request)
        {
            CreateTaskCommand command = new(id, request.Title, request.Description);
            TaskItemDto task = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetJobById), new { id }, task);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateJob(Guid id, UpdateJobRequest request)
        {
            return Ok(await _mediator.Send(new UpdateJobCommand(id, request.Title)));
        }
    }
}
