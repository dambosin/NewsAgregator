using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NewsAgregator.Abstractions.Services;

namespace NewsAgregator.Mvc.Controllers
{
    public class CommentController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ICommentService _commentService;

        public CommentController(
            IMapper mapper, 
            ICommentService commentService)
        {
            _mapper = mapper;
            _commentService = commentService;
        }
        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }
    }
}
