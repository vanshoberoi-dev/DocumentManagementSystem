using DMS.Models.Entities.User;
using DMS.Models.Enums;

namespace DMS.Models.Entities.Document
{
    public class DocumentAuditLog : BaseEntity
    {
        public Guid DocumentId { get; set; }
        public DocumentEntity Document { get; set; } = null!;

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public AuditAction Action { get; set; }
        public string? Details { get; set; }
        public DateTime PerformedAt { get; set; } = DateTime.UtcNow;
    }
}