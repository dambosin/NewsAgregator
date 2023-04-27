using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NewsAgregator.Abstractions.Repository;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Core.Dto;
using NewsAgregator.Data.Entities;
using Serilog;

namespace NewsAgregator.Buisness
{

    public class AricleSrvice : IArticleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Serilog.ILogger _logger;

        public AricleSrvice(
            IUnitOfWork unitOfWork, 
            IMapper mapper,
            ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> CountAsync() => await _unitOfWork.Articles.CountAsync();

        public async Task<IEnumerable<ArticleDto>> GetArticlesByPageAsync(int pageNumber, int pageSize)
        {
            pageNumber = await CheckAndFixPageNumberAsync(pageNumber, pageSize);
            return await _unitOfWork.Articles
                .GetAsQueryable()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(article =>
                    _mapper.Map<ArticleDto>(article))
                .ToListAsync();
        }

        public async Task<ArticleDto> GetArticleDetailAsync(Guid id)
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
            do
            {
                article.Id = Guid.NewGuid();
            } while (!await IsArticleExistAsync(article.Id));
            await _unitOfWork.Articles.AddAsync(_mapper.Map<Article>(article));
            await _unitOfWork.Commit();
            return article.Id;
        }
        public async Task<int> GetPageAmount(int pageSize)
        {
            var articleAmount = await CountAsync();
            return (articleAmount + pageSize - 1) / pageSize;
        }
        
        private async Task<bool> IsArticleExistAsync(Guid id)
        {
            var article = await _unitOfWork.Articles.GetByIdAsync(id);
            return article != null;
        }

        private async Task<int> CheckAndFixPageNumberAsync(int pageNumber, int pageSize)
        {
            var articleAmount = await CountAsync();
            if (pageNumber < 1)
            {
                _logger.Information($"Tried to access {pageNumber}. Value was changed to 1");
                return 1;
            }
            var pageAmount = await GetPageAmount(pageSize);
            if (pageNumber > pageAmount)
            {
                _logger.Information($"Tried to access {pageNumber}, when max is {pageAmount}. Value was changed to {pageAmount}");
                return pageAmount;
            }
            return pageNumber;
        }

    }
}