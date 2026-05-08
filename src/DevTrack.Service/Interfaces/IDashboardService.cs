using DevTrack.Domain.DTOs.Dashboard;

namespace DevTrack.Service.Interfaces;

public interface IDashboardService
{
    Task<DashboardResponse> GetAsync(CancellationToken ct = default);
}
