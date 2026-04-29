using DMS.Data;
using DMS.Models.Contracts.Dashboard;
using DMS.Models.Contracts.Documents;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DMS.Handlers.Dashboard
{
    public class GetDashboardStatsHandler : IRequestHandler<GetDashboardStatsRequest, DashboardStatsResponse>
    {
        private readonly DMSAppDbContext _context;

        public GetDashboardStatsHandler(DMSAppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatsResponse> Handle(GetDashboardStatsRequest request, CancellationToken cancellationToken)
        {
            var documentsQuery = _context.Documents
                .Include(d => d.UploadedBy)
                .Include(d => d.CurrentVersion)
                .Include(d => d.DocumentTags)
                    .ThenInclude(dt => dt.Tag)
                .AsQueryable();

            if (!request.IsAdmin)
            {
                documentsQuery = documentsQuery
                    .Where(d => d.UploadedById == request.UserId);
            }

            var allDocuments = await documentsQuery.ToListAsync(cancellationToken);

            int totalDocuments = allDocuments.Count;

            long totalStorageBytes = allDocuments
                .Where(d => d.CurrentVersion != null)
                .Sum(d => d.CurrentVersion!.FileSize);

            int documentsThisMonth = allDocuments
                .Count(d => d.CreatedAt.Year == DateTime.UtcNow.Year
                    && d.CreatedAt.Month == DateTime.UtcNow.Month);

            int totalUsers = request.IsAdmin
                ? await _context.Users.CountAsync(cancellationToken)
                : 0;

            var recentDocuments = allDocuments
                .OrderByDescending(d => d.CreatedAt)
                .Take(10)
                .Select(d => new DocumentListResponse
                {
                    Id = d.Id,
                    Title = d.Title,
                    UploadedBy = d.UploadedBy?.FullName ?? string.Empty,
                    CreatedAt = d.CreatedAt,
                    AccessCount = d.AccessCount,
                    Tags = d.DocumentTags.Select(dt => dt.Tag?.Name ?? string.Empty).ToList(),
                    ContentType = d.CurrentVersion?.ContentType,
                    FileSize = d.CurrentVersion?.FileSize ?? 0
                }).ToList();

            var mostAccessedDocuments = allDocuments
                .OrderByDescending(d => d.AccessCount)
                .Take(10)
                .Select(d => new DocumentListResponse
                {
                    Id = d.Id,
                    Title = d.Title,
                    UploadedBy = d.UploadedBy?.FullName ?? string.Empty,
                    CreatedAt = d.CreatedAt,
                    AccessCount = d.AccessCount,
                    Tags = d.DocumentTags.Select(dt => dt.Tag?.Name ?? string.Empty).ToList(),
                    ContentType = d.CurrentVersion?.ContentType,
                    FileSize = d.CurrentVersion?.FileSize ?? 0
                }).ToList();

            return new DashboardStatsResponse
            {
                TotalDocuments = totalDocuments,
                TotalStorageBytes = totalStorageBytes,
                DocumentsThisMonth = documentsThisMonth,
                TotalUsers = totalUsers,
                RecentDocuments = recentDocuments,
                MostAccessedDocuments = mostAccessedDocuments
            };
        }
    }
}