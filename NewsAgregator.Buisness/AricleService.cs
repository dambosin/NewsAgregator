using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NewsAgregator.Abstractions.Repository;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Core.Dto;
using System.Security.AccessControl;

namespace NewsAgregator.Buisness
{

    public class AricleSrvice : IArticleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AricleSrvice(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> CountAsync() => await _unitOfWork.Articles.CountAsync();

        public async Task<IEnumerable<ArticleDto>> GetArticlesByPage(int page, int pageSize)
        {
            return await _unitOfWork.Articles
                .GetAsQueryable()
                .Skip(page-1)
                .Take(pageSize)
                .Select(article => 
                    _mapper.Map<ArticleDto>(article))
                .ToListAsync();
        }

        public Task<ArticleDto> GetFullArticleAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}