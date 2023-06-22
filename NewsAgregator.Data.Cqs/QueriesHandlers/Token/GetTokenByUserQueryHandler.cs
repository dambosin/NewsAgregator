    using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NewsAgregator.Data.Cqs.Queris.Token;
using NewsAgregator.Data.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NewsAgregator.Data.Cqs.QueriesHandlers.Token
{
    public class GetTokenByUserIdQueryHandler : IRequestHandler<GetTokenByUserQuery, string>
    {
        private readonly NewsAgregatorContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public GetTokenByUserIdQueryHandler(NewsAgregatorContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        public Task<string> Handle(GetTokenByUserQuery request, CancellationToken cancellationToken)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, request.Login)
            };
            var roles = _context.UserRoles.Where(ur => ur.UserId == request.Id).Select(ur => ur.Role.Name).ToList();

            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role));
            }

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, request.Id.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("D")));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString("R")));

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:SecurityKey"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),

                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var JwtToken = tokenHandler.WriteToken(token);

            return Task.FromResult(JwtToken);
        }
    }
}
