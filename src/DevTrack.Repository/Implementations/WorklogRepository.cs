using DevTrack.Domain.DTOs.Worklogs;
using DevTrack.Domain.Entities;
using DevTrack.Infrastructure.Data;
using DevTrack.Repository.Common;
using DevTrack.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Repository.Implementations;

public class WorklogRepository : IWorklogRepository
{
    private readonly DevTrackDbContext _db;

    public WorklogRepository(DevTrackDbContext db)
    {
        _db = db;
    }

    private IQueryable<Worklog> Query(bool includeDeleted)
    {
        IQueryable<Worklog> q = _db.Worklogs;
        if (includeDeleted) q = q.IgnoreQueryFilters();
        return q;
    }

    public Task<Worklog?> GetByIdAsync(int id, int userId, bool includeDeleted, CancellationToken ct = default)
        => Query(includeDeleted).FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId, ct);

    public async Task<(IReadOnlyList<Worklog> Items, int Total)> ListAsync(int userId, WorklogListQuery query, CancellationToken ct = default)
    {
        var q = Query(query.IncludeDeleted).Where(w => w.UserId == userId);

        if (query.ProjectId.HasValue) q = q.Where(w => w.ProjectId == query.ProjectId);
        if (query.ComponentId.HasValue) q = q.Where(w => w.ComponentId == query.ComponentId);
        if (query.LearningTrackId.HasValue) q = q.Where(w => w.LearningTrackId == query.LearningTrackId);
        if (query.LearningModuleId.HasValue) q = q.Where(w => w.LearningModuleId == query.LearningModuleId);
        if (query.FromDate.HasValue) q = q.Where(w => w.LoggedAt >= query.FromDate);
        if (query.ToDate.HasValue) q = q.Where(w => w.LoggedAt <= query.ToDate);

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(w => w.LoggedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<IReadOnlyList<Worklog>> ListByScopeAsync(int userId, OwnerScope scope, int? take, bool includeDeleted, CancellationToken ct = default)
    {
        if (scope.IsEmpty) return Array.Empty<Worklog>();

        var q = Query(includeDeleted).Where(w => w.UserId == userId &&
            ((w.ProjectId.HasValue && scope.ProjectIds.Contains(w.ProjectId.Value)) ||
             (w.ComponentId.HasValue && scope.ComponentIds.Contains(w.ComponentId.Value)) ||
             (w.LearningTrackId.HasValue && scope.LearningTrackIds.Contains(w.LearningTrackId.Value)) ||
             (w.LearningModuleId.HasValue && scope.LearningModuleIds.Contains(w.LearningModuleId.Value))))
            .OrderByDescending(w => w.LoggedAt);

        return take.HasValue ? await q.Take(take.Value).ToListAsync(ct) : await q.ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Worklog>> ListRecentAsync(int userId, int days, CancellationToken ct = default)
    {
        var cutoff = DateTime.UtcNow.AddDays(-days);
        return await _db.Worklogs
            .Where(w => w.UserId == userId && w.LoggedAt >= cutoff)
            .OrderByDescending(w => w.LoggedAt)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Worklog worklog, CancellationToken ct = default)
        => await _db.Worklogs.AddAsync(worklog, ct);

    public void Update(Worklog worklog) => _db.Worklogs.Update(worklog);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);

    public async Task SoftDeleteByScopeAsync(OwnerScope scope, DateTime utcNow, CancellationToken ct = default)
    {
        if (scope.IsEmpty) return;
        await _db.Worklogs
            .Where(w => !w.IsDeleted &&
                ((w.ProjectId.HasValue && scope.ProjectIds.Contains(w.ProjectId.Value)) ||
                 (w.ComponentId.HasValue && scope.ComponentIds.Contains(w.ComponentId.Value)) ||
                 (w.LearningTrackId.HasValue && scope.LearningTrackIds.Contains(w.LearningTrackId.Value)) ||
                 (w.LearningModuleId.HasValue && scope.LearningModuleIds.Contains(w.LearningModuleId.Value))))
            .ExecuteUpdateAsync(s => s
                .SetProperty(w => w.IsDeleted, true)
                .SetProperty(w => w.DeletedAt, utcNow)
                .SetProperty(w => w.UpdatedAt, utcNow), ct);
    }
}
