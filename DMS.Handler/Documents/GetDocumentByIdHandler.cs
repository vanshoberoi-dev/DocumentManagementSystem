using DMS.Data;
using DMS.Models.Contracts.Documents;
using DMS.Models.Entities.Document;
using DMS.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DMS.Handlers.Documents
{
    public class GetDocumentByIdHandler : IRequestHandler<GetDocumentByIdRequest, DocumentResponse?>
    {
        private readonly DMSAppDbContext _context;

        public GetDocumentByIdHandler(DMSAppDbContext context)
        {
            _context = context;
        }

        public async Task<DocumentResponse?> Handle(GetDocumentByIdRequest request, CancellationToken cancellationToken)
        {
            var document = await _context.Documents
                .Include(d => d.UploadedBy)
                .Include(d => d.CurrentVersion)
                .Include(d => d.DocumentTags)
                    .ThenInclude(dt => dt.Tag)
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (document == null)
                return null;

            document.AccessCount++;
            document.UpdatedAt = DateTime.UtcNow;

            var auditLog = new DocumentAuditLog
            {
                Id = Guid.NewGuid(),
                DocumentId = document.Id,
                UserId = request.RequestedById,
                Action = AuditAction.Viewed,
                Details = $"Document viewed — {document.Title}",
                PerformedAt = DateTime.UtcNow
            };

            await _context.DocumentAuditLogs.AddAsync(auditLog, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new DocumentResponse
            {
                Id = document.Id,
                Title = document.Title,
                Description = document.Description,
                UploadedBy = document.UploadedBy?.FullName ?? string.Empty,
                CreatedAt = document.CreatedAt,
                UpdatedAt = document.UpdatedAt,
                AccessCount = document.AccessCount,
                Tags = document.DocumentTags
                    .Select(dt => dt.Tag?.Name ?? string.Empty)
                    .ToList(),
                CurrentVersion = document.CurrentVersion == null ? null : new VersionResponse
                {
                    Id = document.CurrentVersion.Id,
                    VersionNumber = document.CurrentVersion.VersionNumber,
                    FileName = document.CurrentVersion.FileName,
                    FileSize = document.CurrentVersion.FileSize,
                    ContentType = document.CurrentVersion.ContentType,
                    UploadedBy = document.CurrentVersion.UploadedById,
                    UploadedAt = document.CurrentVersion.CreatedAt
                }
            };
        }
    }
}