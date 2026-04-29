using DMS.Data;
using DMS.Models.Contracts;
using DMS.Models.Contracts.Documents;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DMS.Handlers.Documents
{
    public class GetAllDocumentsHandler : IRequestHandler<GetAllDocumentsRequest, PagedResult<DocumentListResponse>>
    {
        private readonly DMSAppDbContext _context;

        public GetAllDocumentsHandler(DMSAppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<DocumentListResponse>> Handle(GetAllDocumentsRequest request, CancellationToken cancellationToken)
        {
            var query = _context.Documents
                .Include(d => d.UploadedBy)
                .Include(d => d.CurrentVersion)
                .Include(d => d.DocumentTags)
                    .ThenInclude(dt => dt.Tag)
                .AsQueryable();

            if (!request.IsAdmin)
            {
                query = query.Where(d => d.UploadedById == request.RequestedById);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(d =>
                    d.Title.Contains(request.SearchTerm) ||
                    (d.Description != null && d.Description.Contains(request.SearchTerm)));
            }

            if (request.TagId.HasValue)
            {
                query = query.Where(d => d.DocumentTags.Any(dt => dt.TagId == request.TagId.Value));
            }

            int totalCount = await query.CountAsync(cancellationToken);

            var documents = await query
                .OrderByDescending(d => d.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var items = documents.Select(d => new DocumentListResponse
            {
                Id = d.Id,
                Title = d.Title,
                UploadedBy = d.UploadedBy?.FullName ?? string.Empty,
                CreatedAt = d.CreatedAt,
                AccessCount = d.AccessCount,
                Tags = d.DocumentTags.Select(dt => dt.Tag?.Name ?? string.Empty).ToList(),
                ContentType = d.CurrentVersion?.ContentType,
                FileSize = d.CurrentVersion?.FileSize ?? 0
            }).ToList();

            return new PagedResult<DocumentListResponse>
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}