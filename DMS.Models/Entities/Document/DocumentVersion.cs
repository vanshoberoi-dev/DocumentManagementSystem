using DMS.Models.Entities.User;

namespace DMS.Models.Entities.Document
{
    public class DocumentVersion : BaseEntity
    {
        public int VersionNumber { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;

        public Guid DocumentId { get; set; }
        public DocumentEntity? Document { get; set; }

        public string UploadedById { get; set; } = string.Empty;
        public ApplicationUser? UploadedBy { get; set; }
    }
}