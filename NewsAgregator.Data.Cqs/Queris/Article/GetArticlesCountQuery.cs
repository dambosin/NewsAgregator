using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAgregator.Data.Cqs.Queris.Article
{
    public class GetArticlesCountQuery : IRequest<int>
    {
        public int Count { get; set; }
    }
}
