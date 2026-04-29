using DMS.Data;
using DMS.Models.Contracts.Documents;
using DMS.Models.Entities.Document;
using DMS.Models.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DMS.Handlers.Documents
{
    public class UpdateDocumentTagsHandler : IRequestHandler<UpdateDocumentTagsRequest, bool>
    {
        private readonly DMSAppDbContext _context;

        public UpdateDocumentTagsHandler(DMSAppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateDocumentTagsRequest request, CancellationToken cancellationToken)
        {
            var document = await _context.Documents
                .Include(d => d.DocumentTags)
                .FirstOrDefaultAsync(d => d.Id == request.DocumentId, cancellationToken);

            if (document == null)
                return false;

            var existingTagIds = document.DocumentTags
                .Select(dt => dt.TagId)
                .ToList();

            var tagsToRemove = document.DocumentTags
                .Where(dt => !request.TagIds.Contains(dt.TagId))
                .ToList();

            var tagIdsToAdd = request.TagIds
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

            if (tagsToRemove.Any())
            {
                await _context.DocumentAuditLogs.AddAsync(new DocumentAuditLog
                {
                    Id = Guid.NewGuid(),
                    DocumentId = document.Id,
                    UserId = request.UpdatedById,
                    Action = AuditAction.Untagged,
                    Details = $"{tagsToRemove.Count} tag(s) removed",
                    PerformedAt = DateTime.UtcNow
                }, cancellationToken);
            }

            if (tagIdsToAdd.Any())
            {
                await _context.DocumentAuditLogs.AddAsync(new DocumentAuditLog
                {
                    Id = Guid.NewGuid(),
                    DocumentId = document.Id,
                    UserId = request.UpdatedById,
                    Action = AuditAction.Tagged,
                    Details = $"{tagIdsToAdd.Count} tag(s) added",
                    PerformedAt = DateTime.UtcNow
                }, cancellationToken);
            }

            document.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}