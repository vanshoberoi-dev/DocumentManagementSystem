using MediatR;

namespace DMS.Models.Contracts.Documents
{
    public class DeleteDocumentRequest : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string DeletedById { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
    }
}