using MediatR;

namespace DMS.Models.Contracts.Documents
{
    public class UpdateDocumentTagsRequest : IRequest<bool>
    {
        public Guid DocumentId { get; set; }
        public List<Guid> TagIds { get; set; } = new List<Guid>();
        public string UpdatedById { get; set; } = string.Empty;
    }
}