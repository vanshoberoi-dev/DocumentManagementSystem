using MediatR;
using Microsoft.AspNetCore.Http;
using DMS.Models.Contracts.Documents;

namespace DMS.Models.Contracts.Documents
{
    public class UploadDocumentRequest : IRequest<DocumentResponse>
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public IFormFile File { get; set; } = null!;
        public List<Guid> TagIds { get; set; } = new List<Guid>();
        public string UploadedById { get; set; } = string.Empty;
    }
}