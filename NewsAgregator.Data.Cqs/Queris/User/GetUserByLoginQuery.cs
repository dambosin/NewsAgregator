using MediatR;
using NewsAgregator.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAgregator.Data.Cqs.Queris.User
{
    public class GetUserByLoginQuery : IRequest<UserDto>
    {
        public string Login { get; set; }
    }
}
