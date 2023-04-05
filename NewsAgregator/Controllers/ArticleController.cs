using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Mvc.Models;

namespace NewsAgregator.Mvc.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public ArticleController(IArticleService articleService, 
            IConfiguration configuration, 
            IMapper mapper)
        {
            _articleService = articleService;
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
                if(pageInfo.PageNumber > pageInfo.TotalPageCount) pageInfo.PageNumber = pageInfo.TotalPageCount;
                if(pageInfo.PageNumber < 1) pageInfo.PageNumber = 1;
            var articles = await _articleService.GetArticlesByPage(pageInfo.PageNumber, pageInfo.PageSize);
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
    }
}
