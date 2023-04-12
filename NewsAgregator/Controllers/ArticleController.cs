using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Core.Dto;
using NewsAgregator.Mvc.Models.Articles;

namespace NewsAgregator.Mvc.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ICommentService _commentService;
        private readonly ISourceService _sourceSrvice;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public ArticleController(
            IArticleService articleService,
            ICommentService commentService,
            ISourceService sourceService,
            IConfiguration configuration,
            IMapper mapper)
        {
            _articleService = articleService;
            _commentService = commentService;
            _sourceSrvice = sourceService;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var articleCount = await _articleService.CountAsync();

            if (int.TryParse(_configuration["Pagination:Article:DefaultPageSize"], out var pageSize))
            {
                var pageInfo = new PageInfoModel
                {
                    PageSize = pageSize,
                    PageNumber = page,
                    TotalPageCount = (articleCount + pageSize - 1) / pageSize
                };
                if (pageInfo.PageNumber > pageInfo.TotalPageCount) pageInfo.PageNumber = pageInfo.TotalPageCount;
                if (pageInfo.PageNumber < 1) pageInfo.PageNumber = 1;
                var articles = await _articleService.GetArticlesByPageAsync(pageInfo.PageNumber, pageInfo.PageSize);
                var viewModel = new ArticleViewByPageModel
                {
                    Articles = articles.Select(article => _mapper.Map<ArticleModel>(article)).ToList(),
                    PageInfo = pageInfo
                };

                return View(viewModel);
            }
            else
            {
                return StatusCode(500, new { Message = "Can't read configuration data" });
            }
        }

        public async Task<IActionResult> Detail(Guid id)
        {
            var article = await _articleService.GetArticleDetailAsync(id);
            var comments = _commentService.GetCommentsByArticleId(article.Id);
            var viewModel = _mapper.Map<ArticleDetailModel>(article);
            viewModel.Comments = comments.ToList();

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new ArticleCreateModel
            {
                AvailiableSources = _sourceSrvice
                    .GetAvailiableSources()
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
            var articleCreate = _mapper.Map<ArticleCreateDto>(article);
            if (ModelState.IsValid)
            {
                articleCreate.Id = Guid.NewGuid();
                //todo: create positive index calculator
                articleCreate.PositiveIndex = 0;
                articleCreate.Created = DateTime.Now;
                articleCreate.LikesCount = 0;
                await _articleService.CreateAsync(articleCreate);
            }

            return RedirectToAction("Detail", new { id = articleCreate.Id });
        }
    }
}
