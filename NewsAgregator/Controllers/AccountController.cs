using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Mvc.Models.Account;

namespace NewsAgregator.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _accountService;
        private readonly IMapper _mapper;

        public AccountController(
            IUserService accountService,
            IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login([FromForm]LoginModel model)
        {
            return Ok(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register([FromForm]RegisterModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                   
                }
                return View();

            }
            catch (Exception)
            {

                throw;
            }        }
    }
}
