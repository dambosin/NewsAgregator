using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Core.Dto;
using NewsAgregator.Mvc.Models.Accounts;
using System.Security.Claims;

namespace NewsAgregator.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly Serilog.ILogger _logger;

        public AccountController(
            IUserService userService,
            IMapper mapper,
            Serilog.ILogger logger)
        {
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }
        [HttpGet]
        public ActionResult Login() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm]LoginModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new InvalidDataException($"Login model isn't valid. {model}");
                }
                await LoginUser(model);
                return RedirectToAction("index", "home");
            }
            catch ( Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }
        public IActionResult AccessDenied()
        {
            return StatusCode(403);
        }

        private async Task LoginUser(LoginModel model)
        {
            await HttpContext.SignInAsync(
                                CookieAuthenticationDefaults.AuthenticationScheme,
                                new ClaimsPrincipal(await _userService.LoginUser(model.Login, model.Password)));  
            
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync("Cookies");
                return RedirectToAction("index", "home");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new InvalidDataException($"Register model isn't valid. {model}");
                }
                await _userService.RegisterUserAsync(_mapper.Map<UserDto>(model));
                await LoginUser(_mapper.Map<LoginModel>(model));
                return RedirectToAction("index", "home");


            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }
        [HttpGet]
        [Authorize]
        public new IActionResult User([FromRoute]string login)
        {
            var user = _userService.GetUserByLogin(login);
            if(user == null) return NotFound();
            return View();
        }
    }
}
