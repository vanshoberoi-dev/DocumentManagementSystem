using DMS.Data;
using DMS.Models.Contracts.Tags;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DMS.Handlers.Tags
{
    public class CreateTagHandler : IRequestHandler<CreateTagRequest, TagResponse>
    {
        private readonly DMSAppDbContext _context;

        public CreateTagHandler(DMSAppDbContext context)
        {
            _context = context;
        }

        public async Task<TagResponse> Handle(CreateTagRequest request, CancellationToken cancellationToken)
        {
            string normalizedName = request.Name.Trim().ToUpper();

            var existingTag = await _context.Tags
                .FirstOrDefaultAsync(t => t.NormalizedName == normalizedName, cancellationToken);

            if (existingTag != null)
                return new TagResponse
                {
                    Id = existingTag.Id,
                    Name = existingTag.Name
                };

            var tag = new Models.Entities.Tag.Tag
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                NormalizedName = normalizedName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Tags.AddAsync(tag, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new TagResponse
            {
                Id = tag.Id,
                Name = tag.Name
            };
        }
    }
}