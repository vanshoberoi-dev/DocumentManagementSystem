using DMS.Data;
using DMS.Models.Contracts.Tags;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DMS.Handlers.Tags
{
    public class GetAllTagsHandler : IRequestHandler<GetAllTagsRequest, List<TagResponse>>
    {
        private readonly DMSAppDbContext _context;

        public GetAllTagsHandler(DMSAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TagResponse>> Handle(GetAllTagsRequest request, CancellationToken cancellationToken)
        {
            var tags = await _context.Tags
                .OrderBy(t => t.Name)
                .ToListAsync(cancellationToken);

            return tags.Select(t => new TagResponse
            {
                Id = t.Id,
                Name = t.Name
            }).ToList();
        }
    }
}