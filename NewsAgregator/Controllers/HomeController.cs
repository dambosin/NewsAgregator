using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Models;
using NewsAgregator.Mvc.Models.Articles;
using System.Diagnostics;

namespace NewsAgregator.Controllers
{
    public class HomeController : Controller
    {
        private readonly Serilog.ILogger _logger;
        private readonly IArticleService _articleService;
        private readonly IMapper _mapper;

        public HomeController(Serilog.ILogger logger, IArticleService articleService, IMapper mapper)
        {
            _logger = logger;
            _articleService = articleService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var articles = _articleService.GetTopArticles(15).Select(article => _mapper.Map<ArticleModel>(article)).ToList();
            return View(articles);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}