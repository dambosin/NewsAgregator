using AutoMapper;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NewsAgregator.Abstractions;
using NewsAgregator.Abstractions.Repository;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Buisness.Models;
using NewsAgregator.Core.Dto;
using NewsAgregator.Data.Entities;
using Newtonsoft.Json;
using Serilog;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Headers;
using System.Text;


namespace NewsAgregator.Buisness.Services
{
    //todo: rpeort comment- article
    //todo: throw argumeentNullException when argumeent in constructor is null
    //todo: admin report handler page
    public class ArticleSrvice : IArticleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly ISiteParserFactory _parserFactory;
        private readonly IConfiguration _configuration;

        public ArticleSrvice(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger logger,
            ISiteParserFactory parserFactory,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _parserFactory = parserFactory;
            _configuration = configuration;
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
        public async Task<int> LoadFromSourcesAsync()
        {
            var sources = await _unitOfWork.Sources.GetAsQueryable().Select(source => _mapper.Map<SourceDto>(source)).ToListAsync();
            var articles = new List<ArticleDto>();
            foreach (var source in sources)
            {
                articles.AddRange(_parserFactory.GetInstance(source.Name).Parse(source));
            }
            articles = articles.Where(article => !_unitOfWork.Articles.FindBy(art => art.IdOnSite.Equals(article.IdOnSite)).Any()).ToList();
            foreach (var article in articles)
            {
                article.PositiveIndex = await RateTextAsync(ConvertHtmlToText(article.Content!));
            }
            await _unitOfWork.Articles.AddRangeAsync(articles.Select(article => _mapper.Map<Article>(article)));
            await _unitOfWork.CommitAsync();
            return articles.Count;
        }

        private static string ConvertHtmlToText(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var text = "";
            foreach (var node in htmlDoc.DocumentNode.SelectNodes("./p"))
            {
                text += node.InnerText;
            }
            return text;
        }
        public async Task<double> RateTextAsync(string text)
        {
            Dictionary<string, int> lemmaRates = new();
            using (StreamReader reader = new("D:\\Projects\\NewsAgregator\\NewsAgregator\\bin\\Debug\\net6.0\\AFINN-ru.json"))
            {
                string json = await reader.ReadToEndAsync();
                lemmaRates = JsonConvert.DeserializeObject<Dictionary<string, int>>(json);
            }
            var lemmas = new List<LemmaResponse>();
            using (var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7288/")
            })
            {

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var request = new HttpRequestMessage(HttpMethod.Post,
                    $"http://api.ispras.ru/texterra/v1/nlp?targetType=lemma&apikey={_configuration["Secrets:TextErra:ApiKey"]}")
                {
                    Content = new StringContent("[{\"text\":\"" + text + "\"}]",
                                Encoding.UTF8,
                                "application/json")
                };
                var response = await httpClient.SendAsync(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    _logger.Warning(response.ReasonPhrase + " " + response.RequestMessage!.ToString());
                    return -10.0;
                }
                var responseString = await response.Content.ReadAsStringAsync();
                lemmas = JsonConvert.DeserializeObject<List<LemmaResponse>>(responseString);
            }
            double rate = 0;
            foreach (var lemma in lemmas[0].Annotations.Lemma)
            {
                if (!lemmaRates.ContainsKey(lemma.Value)) continue;
                rate += lemmaRates[lemma.Value];
            }
            return rate / lemmas[0].Annotations.Lemma.Count;
        }

        public Task Rate(ArticleDto article)
        {
            throw new NotImplementedException();
        }

        public Task Rate(List<ArticleDto> articles)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(ArticleDto article)
        {
            throw new NotImplementedException();
        }

        public List<ArticleDto> GetByPageWithFilter(int page, int pageSize, Expression<Func<Article, bool>> expression)
        {
            throw new NotImplementedException();
        }
    }
}
