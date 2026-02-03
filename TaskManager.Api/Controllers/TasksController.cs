using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Contracts;
using TaskManager.Application.Features.Tasks.Commands;
using TaskManager.Application.Features.Tasks.Query;
using Utility.Mediator;

namespace TaskManager.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TasksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("search")]
        public async Task<IActionResult> GetAllTasks([FromBody] TasksQuery query)
        {
            return Ok(await _mediator.Send(query));
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPatch]
        [Route("{id}/status")]
        public async Task<IActionResult> UpdateTaskStatus(Guid id, [FromBody] UpdateTaskStatusRequest request)
        {
            return Ok(await _mediator.Send(new UpdateTaskStatusCommand(id, request.Status)));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            return Ok(await _mediator.Send(new DeleteTaskCommand(id)));
        }
    }
}
