using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAgregator.Data.Cqs.QueriesHandlers.Article;
using NewsAgregator.Data.Cqs.Queris.Article;
using NewsAgregator.WebApi.Requests;
using NewsAgregator.WebApi.Responses;

namespace NewsAgregator.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ArticleController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly Serilog.ILogger _logger;
        private readonly IMapper _mapper;

        public ArticleController(
            IMediator mediator,
            Serilog.ILogger logger, 
            IMapper mapper)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet(Name = "GetArticlesByPage")]
        [ProducesResponseType(typeof(ArticlePreviewResponse[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async  Task<IActionResult> GetArticlesByPageAsync([FromQuery]GetArticlesByPageRequest request)
        {
            var articlesCount = await GetArticlesCountAsync();

            if(request.PageSize <= 0 ) return BadRequest($"Incorrect page size: {request.PageSize}. Must be greater than 0");
            var maxPage = Math.Ceiling(articlesCount / (double)request.PageSize);
            if(request.PageNumber > maxPage || request.PageNumber <= 0) return BadRequest($"Incorrect page number: {request.PageNumber}. Must be from 0 to {maxPage}");

            var response = (await _mediator.Send(new GetArticlesByPageQuery()
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            })).Select(dto => _mapper.Map<ArticlePreviewResponse>(dto))
                .ToList();

            return Ok(response);
        }
        [Authorize]
        [HttpGet(Name = "GetArticle")]
        [ProducesResponseType(typeof(ArticleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetArticle([FromQuery] Guid id)
        {
            var article = await _mediator.Send(new GetArticleByIdQuery() { Id = id});
            if (article == null) return NotFound();
            var response = _mapper.Map<ArticleResponse>(article);
            return Ok(response);

        }

        [HttpGet(Name = "GetTotalArticlesCount")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetArticlesCount()
        {
            var response = await GetArticlesCountAsync();
            return Ok(response);
        }

        private async Task<int> GetArticlesCountAsync()
        {
            return await _mediator.Send(new GetArticlesCountQuery());
        }
    }
}