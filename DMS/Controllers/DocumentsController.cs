using DMS.Models.Contracts.Documents;
using DMS.Models.Contracts.Tags;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DMS.Controllers
{
    [Authorize]
    public class DocumentsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DocumentsController> _logger;

        public DocumentsController(IMediator mediator, ILogger<DocumentsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // GET: /Documents
        public async Task<IActionResult> Index(string? searchTerm, Guid? tagId, int page = 1)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            bool isAdmin = User.IsInRole("Admin");

            var result = await _mediator.Send(new GetAllDocumentsRequest
            {
                SearchTerm = searchTerm,
                TagId = tagId,
                RequestedById = userId,
                IsAdmin = isAdmin,
                Page = page,
                PageSize = 7
            });

            var tags = await _mediator.Send(new GetAllTagsRequest());
            ViewBag.Tags = tags;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SelectedTagId = tagId;

            return View(result);
        }

        // GET: /Documents/Details/id
        public async Task<IActionResult> Details(Guid id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var document = await _mediator.Send(new GetDocumentByIdRequest
            {
                Id = id,
                RequestedById = userId
            });

            if (document == null)
            {
                _logger.LogWarning("Document {DocumentId} not found for user {UserId}", id, userId);
                return NotFound();
            }

            var tags = await _mediator.Send(new GetAllTagsRequest());
            ViewBag.Tags = tags;

            return View(document);
        }

        // GET: /Documents/Upload
        public async Task<IActionResult> Upload()
        {
            var tags = await _mediator.Send(new GetAllTagsRequest());
            ViewBag.Tags = tags;
            return View();
        }

        // POST: /Documents/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(UploadDocumentRequest request)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            request.UploadedById = userId;

            if (!ModelState.IsValid)
            {
                var tags = await _mediator.Send(new GetAllTagsRequest());
                ViewBag.Tags = tags;
                return View(request);
            }

            _logger.LogInformation("User {UserId} uploading document {Title}", userId, request.Title);

            var result = await _mediator.Send(request);

            TempData["Success"] = $"Document '{result.Title}' uploaded successfully.";
            return RedirectToAction(nameof(Details), new { id = result.Id });
        }

        // GET: /Documents/Edit/id
        public async Task<IActionResult> Edit(Guid id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            bool isAdmin = User.IsInRole("Admin");

            var document = await _mediator.Send(new GetDocumentByIdRequest
            {
                Id = id,
                RequestedById = userId
            });

            if (document == null)
                return NotFound();

            if (!isAdmin && document.UploadedBy != userId)
            {
                _logger.LogWarning("Unauthorized edit attempt on document {DocumentId} by user {UserId}", id, userId);
                return Forbid();
            }

            var tags = await _mediator.Send(new GetAllTagsRequest());
            ViewBag.Tags = tags;

            var request = new UpdateDocumentRequest
            {
                Id = document.Id,
                Title = document.Title,
                Description = document.Description,
                TagIds = new List<Guid>()
            };

            return View(request);
        }

        // POST: /Documents/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateDocumentRequest request)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            request.Id = id;
            request.UpdatedById = userId;

            if (!ModelState.IsValid)
            {
                var tags = await _mediator.Send(new GetAllTagsRequest());
                ViewBag.Tags = tags;
                return View(request);
            }

            _logger.LogInformation("User {UserId} updating document {DocumentId}", userId, id);

            await _mediator.Send(request);

            TempData["Success"] = "Document updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: /Documents/Delete/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            bool isAdmin = User.IsInRole("Admin");

            _logger.LogInformation("User {UserId} deleting document {DocumentId}", userId, id);

            var result = await _mediator.Send(new DeleteDocumentRequest
            {
                Id = id,
                DeletedById = userId,
                IsAdmin = isAdmin
            });

            if (!result)
            {
                TempData["Error"] = "Document could not be deleted. You may not have permission.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Document deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Documents/Download/id
        public async Task<IActionResult> Download(Guid id, Guid? versionId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var result = await _mediator.Send(new DownloadDocumentRequest
            {
                DocumentId = id,
                VersionId = versionId,
                RequestedById = userId
            });

            if (result == null)
                return NotFound();

            _logger.LogInformation("User {UserId} downloaded document {DocumentId}", userId, id);

            return File(result.FileBytes, result.ContentType, result.FileName);
        }

        // GET: /Documents/Versions/id
        public async Task<IActionResult> Versions(Guid id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var versions = await _mediator.Send(new GetDocumentVersionsRequest
            {
                DocumentId = id,
                RequestedById = userId
            });

            ViewBag.DocumentId = id;
            return View(versions);
        }

        // POST: /Documents/UpdateTags
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTags(Guid documentId, List<Guid> tagIds)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            await _mediator.Send(new UpdateDocumentTagsRequest
            {
                DocumentId = documentId,
                TagIds = tagIds,
                UpdatedById = userId
            });

            TempData["Success"] = "Tags updated successfully.";
            return RedirectToAction(nameof(Details), new { id = documentId });
        }
    }
}