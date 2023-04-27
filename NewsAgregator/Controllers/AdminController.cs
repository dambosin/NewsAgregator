using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NewsAgregator.Mvc.Controllers
{
    
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public AdminController(
            ILogger logger,
            IMapper mapper) 
        { 
            _logger = logger;
            _mapper = mapper;
        }
    }
}
