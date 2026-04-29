using DMS.Models.Contracts.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DMS.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IMediator mediator, ILogger<DashboardController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            bool isAdmin = User.IsInRole("Admin");

            _logger.LogInformation("Dashboard accessed by user {UserId}, IsAdmin: {IsAdmin}", userId, isAdmin);

            var stats = await _mediator.Send(new GetDashboardStatsRequest
            {
                UserId = userId,
                IsAdmin = isAdmin
            });

            return View(stats);
        }
    }
}