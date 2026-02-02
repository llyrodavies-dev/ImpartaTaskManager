using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> UpdateTask(Guid id)
        {
            // Implementation for updating a task
            return Ok();
        }

        [HttpPatch]
        [Route("{id}/status")]
        public async Task<IActionResult> UpdateTaskStatus(Guid id, TaskStatus taskStatus)
        {
            // Implementation for updating task status
            return Ok();
        }
    }
}
