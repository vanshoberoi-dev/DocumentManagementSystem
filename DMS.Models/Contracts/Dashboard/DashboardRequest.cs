using MediatR;

namespace DMS.Models.Contracts.Dashboard
{
    public class GetDashboardStatsRequest : IRequest<DashboardStatsResponse>
    {
        public string UserId { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
    }
}