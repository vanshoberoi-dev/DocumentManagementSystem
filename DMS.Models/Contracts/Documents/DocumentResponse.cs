namespace DMS.Models.Contracts.Documents
{
    public class DocumentResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string UploadedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int AccessCount { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public VersionResponse? CurrentVersion { get; set; }
    }

    public class DocumentListResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string UploadedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int AccessCount { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public string? ContentType { get; set; }
        public long FileSize { get; set; }
    }

    public class VersionResponse
    {
        public Guid Id { get; set; }
        public int VersionNumber { get; set; }
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public string UploadedBy { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }

    public class DownloadDocumentResponse
    {
        public byte[] FileBytes { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }
}