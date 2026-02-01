using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Features.Auth.Commands;
using TaskManager.Application.Features.Jobs.Query;
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
        // create endpoits for the following actions:
        //GET    /api/jobs
        //GET    /api/jobs/{id}
        //POST   /api/jobs
        //POST   /api/jobs/{id}/ tasks
        //PUT    /api/jobs/{ id}

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return Ok(await _mediator.Send(new JobsQuery()));
        }

    }
}
