using DMS.Data;
using DMS.Handlers.Common;
using DMS.Models.Contracts.Documents;
using DMS.Models.Entities.Document;
using DMS.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DMS.Handlers.Documents
{
    public class UploadDocumentHandler : IRequestHandler<UploadDocumentRequest, DocumentResponse>
    {
        private readonly DMSAppDbContext _context;
        private readonly IFileStorageService _fileStorageService;

        public UploadDocumentHandler(DMSAppDbContext context, IFileStorageService fileStorageService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
        }

        public async Task<DocumentResponse> Handle(UploadDocumentRequest request, CancellationToken cancellationToken)
        {
            var (storedFileName, filePath) = await _fileStorageService.SaveFileAsync(request.File);

            var document = new Models.Entities.Document.DocumentEntity
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                UploadedById = request.UploadedById,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Step 1 — Save document first without CurrentVersionId
            await _context.Documents.AddAsync(document, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // Step 2 — Now create version with valid DocumentId
            var version = new DocumentVersion
            {
                Id = Guid.NewGuid(),
                DocumentId = document.Id,
                VersionNumber = 1,
                FileName = request.File.FileName,
                StoredFileName = storedFileName,
                FilePath = filePath,
                FileSize = request.File.Length,
                ContentType = request.File.ContentType,
                UploadedById = request.UploadedById,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.DocumentVersions.AddAsync(version, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // Step 3 — Update CurrentVersionId now that version exists
            document.CurrentVersionId = version.Id;

            if (request.TagIds.Any())
            {
                var tags = await _context.Tags
                    .Where(t => request.TagIds.Contains(t.Id))
                    .ToListAsync(cancellationToken);

                document.DocumentTags = tags.Select(t => new DocumentTag
                {
                    DocumentId = document.Id,
                    TagId = t.Id
                }).ToList();
            }

            var auditLog = new DocumentAuditLog
            {
                Id = Guid.NewGuid(),
                DocumentId = document.Id,
                UserId = request.UploadedById,
                Action = AuditAction.Uploaded,
                Details = $"Version 1 uploaded — {request.File.FileName}",
                PerformedAt = DateTime.UtcNow
            };

            await _context.DocumentAuditLogs.AddAsync(auditLog, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new DocumentResponse
            {
                Id = document.Id,
                Title = document.Title,
                Description = document.Description,
                UploadedBy = request.UploadedById,
                CreatedAt = document.CreatedAt,
                UpdatedAt = document.UpdatedAt,
                AccessCount = document.AccessCount,
                Tags = document.DocumentTags?
                    .Select(dt => dt.Tag?.Name ?? string.Empty)
                    .ToList() ?? new List<string>(),
                CurrentVersion = new VersionResponse
                {
                    Id = version.Id,
                    VersionNumber = version.VersionNumber,
                    FileName = version.FileName,
                    FileSize = version.FileSize,
                    ContentType = version.ContentType,
                    UploadedBy = version.UploadedById,
                    UploadedAt = version.CreatedAt
                }
            };
        }

    }
}