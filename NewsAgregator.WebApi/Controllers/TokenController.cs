using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Data.Cqs.Commands.Token;
using NewsAgregator.Data.Cqs.Queris.Token;
using NewsAgregator.Data.Cqs.Queris.User;
using NewsAgregator.WebApi.Requests;
using NewsAgregator.WebApi.Responses;

namespace NewsAgregator.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TokenController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public TokenController(IMediator mediator, IMapper mapper, IUserService userService)
        {
            _mediator = mediator;
            _mapper = mapper;
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginRequest request)
        {
            var user = await _mediator.Send(new GetUserByLoginQuery()
            {
                Login = request.Login
            });
            if (user == null)
                return BadRequest($"User {request.Login} doesn't exist");
            if (!_userService.IsPasswordCorrect(request.Password, user!.PasswordHash))
                return BadRequest("Invalid user or password");
            var jwtTokenString = await _mediator.Send(new GetTokenByUserQuery()
            {
                Id = user.Id,
                Login = user.Login,
            });

            var refreshToken = Guid.NewGuid();
            await _mediator.Send(new AddRefreshTokenCommand()
            {
                Id = refreshToken,
                UserId = user.Id,
            });
            return Ok(new TokenResponse()
            {
                JwtToken = jwtTokenString,
                RefreshToken = refreshToken.ToString("D"),
                Login = user.Login
            });
        }
    }
}
