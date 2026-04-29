using DMS.Models.Contracts.Documents;

namespace DMS.Models.Contracts.Dashboard
{
    public class DashboardStatsResponse
    {
        public int TotalDocuments { get; set; }
        public long TotalStorageBytes { get; set; }
        public int DocumentsThisMonth { get; set; }
        public int TotalUsers { get; set; }
        public List<DocumentListResponse> RecentDocuments { get; set; } = new List<DocumentListResponse>();
        public List<DocumentListResponse> MostAccessedDocuments { get; set; } = new List<DocumentListResponse>();
    }
}