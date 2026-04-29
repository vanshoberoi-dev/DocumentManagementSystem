using MediatR;
using Microsoft.AspNetCore.Http;

namespace DMS.Models.Contracts.Documents
{
    public class UpdateDocumentRequest : IRequest<DocumentResponse>
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public IFormFile? File { get; set; }
        public List<Guid> TagIds { get; set; } = new List<Guid>();
        public string UpdatedById { get; set; } = string.Empty;
    }
}