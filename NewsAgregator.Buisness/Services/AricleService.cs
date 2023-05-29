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
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using ILogger = Serilog.ILogger;

namespace NewsAgregator.Buisness.Services
{

    public class AricleSrvice : IArticleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly ISiteParserFactory _parserFactory;
        private readonly IConfiguration _configuration;

        public AricleSrvice(
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

        public async Task<IEnumerable<ArticleDto>> GetByPageAsync(int pageNumber, int pageSize)
        {
            return await _unitOfWork.Articles
                .GetAsQueryable()
                .OrderByDescending(article => article.Created)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(article =>
                    _mapper.Map<ArticleDto>(article))
                .ToListAsync();
        }

        public async Task<ArticleDto> GetDetailAsync(Guid id)
        {
            if (!await IsArticleExistAsync(id))
            {
                throw new KeyNotFoundException($"Article with id {id} doesn't exist");
            }
            var article = await _unitOfWork.Articles.GetByIdAsync(id);
            return _mapper.Map<ArticleDto>(article);
        }

        public async Task<Guid> CreateAsync(ArticleCreateDto article)
        {
            await _unitOfWork.Articles.AddAsync(_mapper.Map<Article>(article));
            await _unitOfWork.CommitAsync();
            return article.Id;
        }
        private async Task<bool> IsArticleExistAsync(Guid id)
        {
            var article = await _unitOfWork.Articles.GetByIdAsync(id);
            return article != null;
        }
        public async Task<int> LoadFromSourcesAsync()
        {
            var sources = await _unitOfWork.Sources.GetAsQueryable().Select(source => _mapper.Map<SourceDto>(source)).ToListAsync();
            var articles = new List<ArticleCreateDto>();
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
    }
}
