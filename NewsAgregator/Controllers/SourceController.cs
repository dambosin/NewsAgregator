using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Core.Dto;
using NewsAgregator.Mvc.Models.Sources;

namespace NewsAgregator.Mvc.Controllers
{
    public class SourceController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ISourceService _sourceService;

        public SourceController(IMapper mapper, ISourceService sourceService)
        {
            _mapper = mapper;
            _sourceService = sourceService;
        }

        public IActionResult Index()
        {
            return View(_sourceService
                .GetSources()
                .Select(source => _mapper.Map<SourceModel>(source))
                .ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(SourceCreateModel source)
        {
            if (ModelState.IsValid)
            {
                var sourceCreate = _mapper.Map<SourceDto>(source);
                sourceCreate.Id = Guid.NewGuid();
                await _sourceService.Create(sourceCreate);
            }
            return RedirectToAction("Index");
        }
    }
}
