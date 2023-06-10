using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Core.Dto;
using NewsAgregator.Mvc.Models.Roles;
using NewsAgregator.Mvc.Models.Users;

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
        private readonly IUserService _userService;

        public AdminController(
            Serilog.ILogger logger,
            IMapper mapper,
            IRoleService roleService,
            IUserService userService)
        {
            _logger = logger;
            _mapper = mapper;
            _roleService = roleService;
            _userService = userService;
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
        public IActionResult ManageUsers()
        {
            try
            {
                var model = new UsersViewModel { Users = _userService.GetUsers()};
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
                await _roleService.CreateAsync(_mapper.Map<RoleDto>(model));
                return RedirectToAction("ManageRoles");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> UpdateRole([FromRoute]Guid id)
        {
            try
            {
                var role = await _roleService.GetByIdAsync(id);
                return View(_mapper.Map<RoleUpdateModel>(role));
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateRole([FromForm]RoleUpdateModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new InvalidDataException();
                }
                await _roleService.UpdateAsync(_mapper.Map<RoleDto>(model));
                return RedirectToAction("ManageRoles");
            }
            catch(Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRole([FromRoute]Guid id)
        {
            try
            {
                var role = await _roleService.GetByIdAsync(id) ?? throw new InvalidDataException($"Role with id {id} is not exist");
                await _roleService.DeleteAsync(id);
                return RedirectToAction("ManageRoles");
            }catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }
    }
}
