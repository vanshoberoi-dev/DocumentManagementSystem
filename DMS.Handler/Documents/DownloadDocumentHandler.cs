using DMS.Data;
using DMS.Handlers.Common;
using DMS.Models.Contracts.Documents;
using DMS.Models.Entities.Document;
using DMS.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DMS.Handlers.Documents
{
    public class DownloadDocumentHandler : IRequestHandler<DownloadDocumentRequest, DownloadDocumentResponse?>
    {
        private readonly DMSAppDbContext _context;
        private readonly IFileStorageService _fileStorageService;

        public DownloadDocumentHandler(DMSAppDbContext context, IFileStorageService fileStorageService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
        }

        public async Task<DownloadDocumentResponse?> Handle(DownloadDocumentRequest request, CancellationToken cancellationToken)
        {
            var document = await _context.Documents
                .Include(d => d.CurrentVersion)
                .FirstOrDefaultAsync(d => d.Id == request.DocumentId, cancellationToken);

            if (document == null)
                return null;

            DocumentVersion? version;

            if (request.VersionId.HasValue)
            {
                version = await _context.DocumentVersions
                    .FirstOrDefaultAsync(v => v.Id == request.VersionId.Value
                        && v.DocumentId == request.DocumentId, cancellationToken);
            }
            else
            {
                version = document.CurrentVersion;
            }

            if (version == null)
                return null;

            document.AccessCount++;
            document.UpdatedAt = DateTime.UtcNow;

            var auditLog = new DocumentAuditLog
            {
                Id = Guid.NewGuid(),
                DocumentId = document.Id,
                UserId = request.RequestedById,
                Action = AuditAction.Downloaded,
                Details = $"Version {version.VersionNumber} downloaded — {version.FileName}",
                PerformedAt = DateTime.UtcNow
            };

            await _context.DocumentAuditLogs.AddAsync(auditLog, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var fileBytes = await _fileStorageService.ReadFileAsync(version.FilePath);

            return new DownloadDocumentResponse
            {
                FileBytes = fileBytes,
                FileName = version.FileName,
                ContentType = version.ContentType
            };
        }
    }
}