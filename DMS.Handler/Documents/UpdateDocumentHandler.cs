using DMS.Data;
using DMS.Handlers.Common;
using DMS.Models.Contracts.Documents;
using DMS.Models.Entities.Document;
using DMS.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DMS.Handlers.Documents
{
    public class UpdateDocumentHandler : IRequestHandler<UpdateDocumentRequest, DocumentResponse>
    {
        private readonly DMSAppDbContext _context;
        private readonly IFileStorageService _fileStorageService;

        public UpdateDocumentHandler(DMSAppDbContext context, IFileStorageService fileStorageService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
        }

        public async Task<DocumentResponse> Handle(UpdateDocumentRequest request, CancellationToken cancellationToken)
        {
            var document = await _context.Documents
                .Include(d => d.UploadedBy)
                .Include(d => d.CurrentVersion)
                .Include(d => d.DocumentTags)
                    .ThenInclude(dt => dt.Tag)
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (document == null)
                throw new KeyNotFoundException($"Document with Id {request.Id} not found.");

            document.Title = request.Title;
            document.Description = request.Description;
            document.UpdatedAt = DateTime.UtcNow;

            if (request.File != null)
            {
                int nextVersionNumber = await _context.DocumentVersions
                    .Where(v => v.DocumentId == document.Id)
                    .MaxAsync(v => v.VersionNumber, cancellationToken) + 1;

                var (storedFileName, filePath) = await _fileStorageService.SaveFileAsync(request.File);

                var newVersion = new DocumentVersion
                {
                    Id = Guid.NewGuid(),
                    DocumentId = document.Id,
                    VersionNumber = nextVersionNumber,
                    FileName = request.File.FileName,
                    StoredFileName = storedFileName,
                    FilePath = filePath,
                    FileSize = request.File.Length,
                    ContentType = request.File.ContentType,
                    UploadedById = request.UpdatedById,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                document.CurrentVersionId = newVersion.Id;
                await _context.DocumentVersions.AddAsync(newVersion, cancellationToken);

                var versionAuditLog = new DocumentAuditLog
                {
                    Id = Guid.NewGuid(),
                    DocumentId = document.Id,
                    UserId = request.UpdatedById,
                    Action = AuditAction.Uploaded,
                    Details = $"Version {nextVersionNumber} uploaded — {request.File.FileName}",
                    PerformedAt = DateTime.UtcNow
                };

                await _context.DocumentAuditLogs.AddAsync(versionAuditLog, cancellationToken);
            }

            var existingTagIds = document.DocumentTags.Select(dt => dt.TagId).ToList();
            var newTagIds = request.TagIds;

            var tagsToRemove = document.DocumentTags
                .Where(dt => !newTagIds.Contains(dt.TagId))
                .ToList();

            var tagIdsToAdd = newTagIds
                .Where(id => !existingTagIds.Contains(id))
                .ToList();

            foreach (var tag in tagsToRemove)
                _context.DocumentTags.Remove(tag);

            foreach (var tagId in tagIdsToAdd)
                await _context.DocumentTags.AddAsync(new DocumentTag
                {
                    DocumentId = document.Id,
                    TagId = tagId
                }, cancellationToken);

            var auditLog = new DocumentAuditLog
            {
                Id = Guid.NewGuid(),
                DocumentId = document.Id,
                UserId = request.UpdatedById,
                Action = AuditAction.Updated,
                Details = $"Document updated — {document.Title}",
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