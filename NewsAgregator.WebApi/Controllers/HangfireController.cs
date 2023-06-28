using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAgregator.Data.Cqs.Commands.Article;
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
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public  IActionResult Init()
        {
            RecurringJob.AddOrUpdate(
                "LoadArticlesDtf",
                () => LoadFromDtf(),
                "0 0,4,8,12,16,20 * * *");

            RecurringJob.AddOrUpdate(
                "LoadArticlesOnliner",
                () => LoadFromOnliner(),
                "0 * * * *");

            RecurringJob.AddOrUpdate(
                "RateArticles",
                () => RateArticles(),
                "5 * * * *");
            return Ok();
        }
        [NonAction]
        public async Task LoadFromDtf()
        {
            await _mediator.Send(new LoadArticlesCommand()
            {
                SourceName = "Dtf"
            });
        }
        [NonAction]
        public async Task LoadFromOnliner()
        {
            await _mediator.Send(new LoadArticlesCommand()
            {
                SourceName = "Onliner"
            });
        }
        [NonAction]
        public async Task RateArticles()
        {
            await _mediator.Send(new RateArticlesCommand());
        }
    }
}
