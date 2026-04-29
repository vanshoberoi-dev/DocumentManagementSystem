using MediatR;

namespace DMS.Models.Contracts.Documents
{
    public class DownloadDocumentRequest : IRequest<DownloadDocumentResponse?>
    {
        public Guid DocumentId { get; set; }
        public Guid? VersionId { get; set; }
        public string RequestedById { get; set; } = string.Empty;
    }
}