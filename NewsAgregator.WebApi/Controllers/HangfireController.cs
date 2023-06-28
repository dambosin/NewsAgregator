using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAgregator.Data.Cqs.Commands.Article;
using NewsAgregator.Data.Cqs.Commands.Hangfire;
using NewsAgregator.WebApi.Responses;
using Serilog;

namespace NewsAgregator.WebApi.Controllers
{    
    [ApiController]
    [Route("[controller]/[action]")]
    public class JobsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly Serilog.ILogger _logger;

        public JobsController(IMediator mediator, Serilog.ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        [HttpGet(Name = "Init")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public  IActionResult Init()
        {
            _mediator.Send(new HangfireInitCommand());
            return Ok();
        }
    }
}
