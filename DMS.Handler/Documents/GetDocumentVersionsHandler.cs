using DMS.Data;
using DMS.Models.Contracts.Documents;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DMS.Handlers.Documents
{
    public class GetDocumentVersionsHandler : IRequestHandler<GetDocumentVersionsRequest, List<VersionResponse>>
    {
        private readonly DMSAppDbContext _context;

        public GetDocumentVersionsHandler(DMSAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<VersionResponse>> Handle(GetDocumentVersionsRequest request, CancellationToken cancellationToken)
        {
            var document = await _context.Documents
                .FirstOrDefaultAsync(d => d.Id == request.DocumentId, cancellationToken);

            if (document == null)
                return new List<VersionResponse>();

            var versions = await _context.DocumentVersions
                .Include(v => v.UploadedBy)
                .Where(v => v.DocumentId == request.DocumentId)
                .OrderBy(v => v.VersionNumber)
                .ToListAsync(cancellationToken);

            return versions.Select(v => new VersionResponse
            {
                Id = v.Id,
                VersionNumber = v.VersionNumber,
                FileName = v.FileName,
                FileSize = v.FileSize,
                ContentType = v.ContentType,
                UploadedBy = v.UploadedBy?.FullName ?? string.Empty,
                UploadedAt = v.CreatedAt
            }).ToList();
        }
    }
}