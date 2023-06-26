using AutoMapper;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Configuration;
using NewsAgregator.Abstractions;
using NewsAgregator.Abstractions.Repository;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Core.Dto;
using NewsAgregator.Data.Entities;
using Serilog;
using System.Linq;
using System.Linq.Expressions;
using System.Security;
using System.Text.RegularExpressions;

namespace NewsAgregator.Buisness.Services
{
    //todo: rpeort comment- article
    //todo: throw argumeentNullException when argumeent in constructor is null
    //todo: admin report handler page
    //todo: article filter
    public class ArticleSrvice : IArticleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly ISiteParserFactory _parserFactory;
        private readonly IConfiguration _configuration;
        private readonly ISourceService _sourceService;
        private readonly IRateService _rateService;

        public ArticleSrvice(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger logger,
            ISiteParserFactory parserFactory,
            IConfiguration configuration,
            ISourceService sourceService,
            IRateService rateService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _parserFactory = parserFactory;
            _configuration = configuration;
            _sourceService = sourceService;
            _rateService = rateService;
        }

        public async Task<int> CountAsync() => await _unitOfWork.Articles.CountAsync();

        public List<ArticleDto> GetByPage(int pageNumber, int pageSize)
        {
            var articles = _unitOfWork.Articles.GetAsQueryable();
            if (pageSize < 1) throw new ArgumentOutOfRangeException($"Page size must be 1 or higher. pageSize = {pageSize}");
            
            if (pageNumber < 1) 
                throw new ArgumentOutOfRangeException($"Page number must be 1 or higher. pageNumber = {pageNumber}");

            var maxPage = Math.Ceiling(articles.Count() / (double)pageSize);
            if (maxPage < pageNumber) 
                throw new ArgumentOutOfRangeException($"Page number must be same or lower than last pagee. pageNumber = {pageNumber}, last page = {maxPage}");
            
            return articles
                .OrderByDescending(article => article.Created)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(article =>
                    _mapper.Map<ArticleDto>(article))
                .ToList();
        }

        public async Task<ArticleDto> GetArticleAsync(Guid id)
        {
            var article = await _unitOfWork.Articles.GetByIdAsync(id);
            if (article == null)
                throw new ArgumentException($"Article with id = {id} doesn't exist");
            return _mapper.Map<ArticleDto>(article);
        }

        public async Task<Guid> CreateAsync(ArticleDto article)
        {
            await _unitOfWork.Articles.AddAsync(_mapper.Map<Article>(article));
            await _unitOfWork.CommitAsync();
            return article.Id;
        }
        public Task RemoveAsync(Guid id)
        {
            throw new NotImplementedException();
        }
        public Task UpdateAsync(ArticleDto article)
        {
            throw new NotImplementedException();
        }
        public async Task<int> LoadFromSourcesAsync()
        {
            var sources = _sourceService.GetSources();
            foreach(var source in sources)
            {
                var articles = _parserFactory.GetInstance(source.Name).Parse(source);
                var allArticlesOriginalId = GetArticlesOriginalIds();
                articles = articles.Where(article => !allArticlesOriginalId.Any(id => id.Equals(article.IdOnSite))).ToList();
                await _unitOfWork.Articles.AddRangeAsync(articles
                    .Select(article => _mapper.Map<Article>(article)));
            }
            return await _unitOfWork.CommitAsync();
        }
        public async Task RateArticlesAsync()
        {
            var articlesToRate = _unitOfWork.Articles
                .GetAsQueryable()
                .Where(article => article.PositiveIndex == -10)
                .ToList();
            articlesToRate = await Rate(articlesToRate);
            foreach(var article in articlesToRate)
            {
                _unitOfWork.Articles.Update(article);
            }
            await _unitOfWork.CommitAsync();
        }
       
        public async Task<double> Rate(Article article)
        {
            return await _rateService.Rate(article.PlainText);
        }
        public async Task<List<Article>> Rate(List<Article> articles)
        {
            foreach (var article in articles)
            {
                article.PositiveIndex = await Rate(article);
            }
            return articles;
        }
        public List<ArticleDto> GetByPageWithFilter(int page, int pageSize, Expression<Func<Article, bool>> expression)
        {
            throw new NotImplementedException();
        }
        
        private List<string> GetArticlesOriginalIds()
        {
            return _unitOfWork.Articles.GetAsQueryable().Select(article => article.IdOnSite).ToList();
        }
    }
}