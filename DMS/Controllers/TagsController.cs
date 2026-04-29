using DMS.Models.Contracts.Tags;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMS.Controllers
{
    [Authorize]
    public class TagsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TagsController> _logger;

        public TagsController(IMediator mediator, ILogger<TagsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // GET: /Tags
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var tags = await _mediator.Send(new GetAllTagsRequest());
            return View(tags);
        }

        // POST: /Tags/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateTagRequest request)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Tag name is required.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogInformation("Admin creating tag {TagName}", request.Name);

            var result = await _mediator.Send(request);

            TempData["Success"] = $"Tag '{result.Name}' created successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}