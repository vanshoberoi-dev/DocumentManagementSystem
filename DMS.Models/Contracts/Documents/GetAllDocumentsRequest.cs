using MediatR;

namespace DMS.Models.Contracts.Documents
{
    public class GetAllDocumentsRequest : IRequest<PagedResult<DocumentListResponse>>
    {
        public string? SearchTerm { get; set; }
        public Guid? TagId { get; set; }
        public string? RequestedById { get; set; }
        public bool IsAdmin { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}