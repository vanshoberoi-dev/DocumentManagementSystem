using MediatR;

namespace DMS.Models.Contracts.Documents
{
    public class GetDocumentByIdRequest : IRequest<DocumentResponse?>
    {
        public Guid Id { get; set; }
        public string RequestedById { get; set; } = string.Empty;
    }
}