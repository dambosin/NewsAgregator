using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAgregator.Data.Cqs.Queris.Token
{
    public class GetTokenByUserQuery : IRequest<string>
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
    }
}
