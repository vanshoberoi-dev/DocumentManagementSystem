using MediatR;

namespace DMS.Models.Contracts.Documents
{
    public class GetDocumentVersionsRequest : IRequest<List<VersionResponse>>
    {
        public Guid DocumentId { get; set; }
        public string RequestedById { get; set; } = string.Empty;
    }
}