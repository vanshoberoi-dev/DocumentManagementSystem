using DMS.Data;
using DMS.Models.Contracts.Documents;
using DMS.Models.Entities.Document;
using DMS.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DMS.Handlers.Documents
{
    public class DeleteDocumentHandler : IRequestHandler<DeleteDocumentRequest, bool>
    {
        private readonly DMSAppDbContext _context;

        public DeleteDocumentHandler(DMSAppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteDocumentRequest request, CancellationToken cancellationToken)
        {
            var document = await _context.Documents
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

            if (document == null)
                return false;

            if (!request.IsAdmin && document.UploadedById != request.DeletedById)
                return false;

            document.IsDeleted = true;
            document.UpdatedAt = DateTime.UtcNow;

            var auditLog = new DocumentAuditLog
            {
                Id = Guid.NewGuid(),
                DocumentId = document.Id,
                UserId = request.DeletedById,
                Action = AuditAction.Deleted,
                Details = $"Document soft deleted — {document.Title}",
                PerformedAt = DateTime.UtcNow
            };

            await _context.DocumentAuditLogs.AddAsync(auditLog, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}