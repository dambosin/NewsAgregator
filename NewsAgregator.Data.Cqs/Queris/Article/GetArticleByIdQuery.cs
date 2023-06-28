using MediatR;
using NewsAgregator.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAgregator.Data.Cqs.QueriesHandlers.Article
{
    public class GetArticleByIdQuery : IRequest<ArticleDto>
    {
        public Guid Id { get; set; }
    }
}
