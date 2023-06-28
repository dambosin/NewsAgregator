using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Core.Dto;
using NewsAgregator.Mvc.Models.Articles;

namespace NewsAgregator.Mvc.Controllers
{
    [Authorize]
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ICommentService _commentService;
        private readonly ISourceService _sourceSrvice;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly Serilog.ILogger _logger;

        public ArticleController(
            IArticleService articleService,
            ICommentService commentService,
            ISourceService sourceService,
            IConfiguration configuration,
            IMapper mapper,
            Serilog.ILogger logger)
        {
            _articleService = articleService;
            _commentService = commentService;
            _sourceSrvice = sourceService;
            _configuration = configuration;
            _mapper = mapper;
            _logger = logger;

        }

        public async Task<IActionResult> Index([FromQuery]int page = 1)
        {
            try
            {
                if (!int.TryParse(_configuration["Pagination:Article:DefaultPageSize"], out var pageSize))
                {
                    throw new Exception("Cant't read configuration data");
                }
                var pageInfo = new PageInfoModel
                {
                    PageSize = pageSize,
                    PageNumber = page,
                    PageAmount = (await _articleService.CountAsync() + pageSize - 1) / pageSize 
                };
                var articles = _articleService.GetByPage(pageInfo.PageNumber, pageInfo.PageSize);
                var viewModel = new ArticleViewByPageModel
                {
                    Articles = articles.Select(article => _mapper.Map<ArticleModel>(article)).ToList(),
                    PageInfo = pageInfo
                };

                return View(viewModel);

            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                throw;
            }
        }

        public async Task<IActionResult> Detail([FromRoute]Guid id)
        {
            try
            {
                var article = await _articleService.GetArticleAsync(id);
                var viewModel = _mapper.Map<ArticleDetailModel>(article);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                throw;
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new ArticleCreateModel
            {
                AvailiableSources = _sourceSrvice
                    .GetSources()
                    .Select(source =>
                        new SelectListItem
                        {
                            Value = source.Id.ToString(),
                            Text = source.Name
                        })
                    .ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ArticleCreateModel article)
        {
            var articleCreate = _mapper.Map<ArticleDto>(article);
            if (ModelState.IsValid)
            {
                await _articleService.CreateAsync(articleCreate);
            }

            return RedirectToAction("Detail", new { id = articleCreate.Id });
        }
        [HttpPost]
        public async Task<IActionResult> LoadArticles()
        {
            await _articleService.LoadFromSourcesAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> RateArticles()
        {
            await _articleService.RateArticlesAsync();
            return RedirectToAction("Index");
        }

    }
}
