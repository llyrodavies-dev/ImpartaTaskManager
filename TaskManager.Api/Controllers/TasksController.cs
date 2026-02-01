using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            // Implementation for getting all tasks
            return Ok();
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
        public async Task<IActionResult> UpdateTaskStatus(Guid id)
        {
            // Implementation for updating task status
            return Ok();
        }
    }
}
