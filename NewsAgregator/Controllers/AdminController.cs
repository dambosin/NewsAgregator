using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Core.Dto;
using NewsAgregator.Mvc.Models.Roles;

namespace NewsAgregator.Mvc.Controllers
{
    // Todo: correct redirect
    // Todo: managers (add/update/deleete)
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly Serilog.ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IRoleService _roleService;

        public AdminController(
            Serilog.ILogger logger,
            IMapper mapper,
            IRoleService roleService) 
        { 
            _logger = logger;
            _mapper = mapper;
            _roleService = roleService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ManageRoles()
        {
            try
            {
                var model = new RolesViewModel { Roles = _roleService.GetRoles() };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleCreateModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new InvalidDataException("Model is not correct");
                }
                await _roleService.CreateAsyc(_mapper.Map<RoleDto>(model));
                return RedirectToAction("ManageRoles");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }
    }
}
