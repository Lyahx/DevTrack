using DevTrack.Domain.DTOs.Worklogs;
using DevTrack.Domain.Entities;
using DevTrack.Repository.Common;

namespace DevTrack.Repository.Interfaces;

public interface IWorklogRepository
{
    Task<Worklog?> GetByIdAsync(int id, int userId, bool includeDeleted, CancellationToken ct = default);
    Task<(IReadOnlyList<Worklog> Items, int Total)> ListAsync(int userId, WorklogListQuery query, CancellationToken ct = default);
    Task<IReadOnlyList<Worklog>> ListByScopeAsync(int userId, OwnerScope scope, int? take, bool includeDeleted, CancellationToken ct = default);
    Task<IReadOnlyList<Worklog>> ListRecentAsync(int userId, int days, CancellationToken ct = default);
    Task SoftDeleteByScopeAsync(OwnerScope scope, DateTime utcNow, CancellationToken ct = default);
    Task AddAsync(Worklog worklog, CancellationToken ct = default);
    void Update(Worklog worklog);
    Task SaveChangesAsync(CancellationToken ct = default);
}
