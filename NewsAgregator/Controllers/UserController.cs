using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Mvc.Models.Users;

namespace NewsAgregator.Mvc.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly Serilog.ILogger _logger;

        public UserController(
            IUserService userService,
            IMapper mapper,
            Serilog.ILogger logger)
        {
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }
        [HttpGet]
        public new async Task<IActionResult> User(Guid id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null) return NotFound();
                return View(_mapper.Map<UserModel>(user));
            }catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }
    }
}
