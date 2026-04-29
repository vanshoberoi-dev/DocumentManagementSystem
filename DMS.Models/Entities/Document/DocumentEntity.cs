using DMS.Models.Entities.User;

namespace DMS.Models.Entities.Document
{
    public class DocumentEntity : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsDeleted { get; set; } = false;
        public int AccessCount { get; set; } = 0;

        public string UploadedById { get; set; } = string.Empty;
        public ApplicationUser? UploadedBy { get; set; }

        public Guid? CurrentVersionId { get; set; }
        public DocumentVersion? CurrentVersion { get; set; }

        public ICollection<DocumentVersion> Versions { get; set; } = new List<DocumentVersion>();
        public ICollection<DocumentTag> DocumentTags { get; set; } = new List<DocumentTag>();
        public ICollection<DocumentAuditLog> AuditLogs { get; set; } = new List<DocumentAuditLog>();
    }
}