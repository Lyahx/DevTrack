using DevTrack.Domain.DTOs.Decisions;
using DevTrack.Domain.Entities;
using DevTrack.Infrastructure.Data;
using DevTrack.Repository.Common;
using DevTrack.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Repository.Implementations;

public class DecisionRepository : IDecisionRepository
{
    private readonly DevTrackDbContext _db;

    public DecisionRepository(DevTrackDbContext db)
    {
        _db = db;
    }

    private IQueryable<Decision> Query(bool includeDeleted)
    {
        IQueryable<Decision> q = _db.Decisions;
        if (includeDeleted) q = q.IgnoreQueryFilters();
        return q;
    }

    public Task<Decision?> GetByIdAsync(int id, int userId, bool includeDeleted, CancellationToken ct = default)
        => Query(includeDeleted).FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId, ct);

    public async Task<(IReadOnlyList<Decision> Items, int Total)> ListAsync(int userId, DecisionListQuery query, CancellationToken ct = default)
    {
        var q = Query(query.IncludeDeleted).Where(d => d.UserId == userId);

        if (query.ProjectId.HasValue) q = q.Where(d => d.ProjectId == query.ProjectId);
        if (query.ComponentId.HasValue) q = q.Where(d => d.ComponentId == query.ComponentId);
        if (query.LearningTrackId.HasValue) q = q.Where(d => d.LearningTrackId == query.LearningTrackId);
        if (query.LearningModuleId.HasValue) q = q.Where(d => d.LearningModuleId == query.LearningModuleId);

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(d => d.DecidedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<IReadOnlyList<Decision>> ListByScopeAsync(int userId, OwnerScope scope, int? take, bool includeDeleted, CancellationToken ct = default)
    {
        if (scope.IsEmpty) return Array.Empty<Decision>();

        var q = Query(includeDeleted).Where(d => d.UserId == userId &&
            ((d.ProjectId.HasValue && scope.ProjectIds.Contains(d.ProjectId.Value)) ||
             (d.ComponentId.HasValue && scope.ComponentIds.Contains(d.ComponentId.Value)) ||
             (d.LearningTrackId.HasValue && scope.LearningTrackIds.Contains(d.LearningTrackId.Value)) ||
             (d.LearningModuleId.HasValue && scope.LearningModuleIds.Contains(d.LearningModuleId.Value))))
            .OrderByDescending(d => d.DecidedAt);

        return take.HasValue ? await q.Take(take.Value).ToListAsync(ct) : await q.ToListAsync(ct);
    }

    public async Task AddAsync(Decision decision, CancellationToken ct = default)
        => await _db.Decisions.AddAsync(decision, ct);

    public void Update(Decision decision) => _db.Decisions.Update(decision);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);

    public async Task SoftDeleteByScopeAsync(OwnerScope scope, DateTime utcNow, CancellationToken ct = default)
    {
        if (scope.IsEmpty) return;
        await _db.Decisions
            .Where(d => !d.IsDeleted &&
                ((d.ProjectId.HasValue && scope.ProjectIds.Contains(d.ProjectId.Value)) ||
                 (d.ComponentId.HasValue && scope.ComponentIds.Contains(d.ComponentId.Value)) ||
                 (d.LearningTrackId.HasValue && scope.LearningTrackIds.Contains(d.LearningTrackId.Value)) ||
                 (d.LearningModuleId.HasValue && scope.LearningModuleIds.Contains(d.LearningModuleId.Value))))
            .ExecuteUpdateAsync(s => s
                .SetProperty(d => d.IsDeleted, true)
                .SetProperty(d => d.DeletedAt, utcNow)
                .SetProperty(d => d.UpdatedAt, utcNow), ct);
    }
}
