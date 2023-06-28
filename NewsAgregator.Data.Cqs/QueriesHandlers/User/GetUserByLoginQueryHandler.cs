using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NewsAgregator.Core.Dto;
using NewsAgregator.Data.Cqs.Queris.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAgregator.Data.Cqs.QueriesHandlers.User
{
    public class GetUserByLoginQueryHandler : IRequestHandler<GetUserByLoginQuery, UserDto>
    {
        private readonly NewsAgregatorContext _context;
        private readonly IMapper _mapper;

        public GetUserByLoginQueryHandler(NewsAgregatorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(GetUserByLoginQuery request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<UserDto>(await _context.Users.SingleOrDefaultAsync(u => u.Login.Equals(request.Login), cancellationToken));
            return user;
        }
    }
}
