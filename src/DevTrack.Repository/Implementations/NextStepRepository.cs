using DevTrack.Domain.DTOs.NextSteps;
using DevTrack.Domain.Entities;
using DevTrack.Domain.Enums;
using DevTrack.Infrastructure.Data;
using DevTrack.Repository.Common;
using DevTrack.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Repository.Implementations;

public class NextStepRepository : INextStepRepository
{
    private readonly DevTrackDbContext _db;

    public NextStepRepository(DevTrackDbContext db)
    {
        _db = db;
    }

    private IQueryable<NextStep> Query(bool includeDeleted)
    {
        IQueryable<NextStep> q = _db.NextSteps;
        if (includeDeleted) q = q.IgnoreQueryFilters();
        return q;
    }

    public Task<NextStep?> GetByIdAsync(int id, int userId, bool includeDeleted, CancellationToken ct = default)
        => Query(includeDeleted).FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId, ct);

    public async Task<(IReadOnlyList<NextStep> Items, int Total)> ListAsync(int userId, NextStepListQuery query, CancellationToken ct = default)
    {
        var q = Query(query.IncludeDeleted).Where(s => s.UserId == userId);

        if (query.ProjectId.HasValue) q = q.Where(s => s.ProjectId == query.ProjectId);
        if (query.ComponentId.HasValue) q = q.Where(s => s.ComponentId == query.ComponentId);
        if (query.LearningTrackId.HasValue) q = q.Where(s => s.LearningTrackId == query.LearningTrackId);
        if (query.LearningModuleId.HasValue) q = q.Where(s => s.LearningModuleId == query.LearningModuleId);
        if (query.IsCompleted.HasValue) q = q.Where(s => s.IsCompleted == query.IsCompleted.Value);
        if (query.Priority.HasValue) q = q.Where(s => s.Priority == query.Priority.Value);

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(s => s.Priority).ThenBy(s => s.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<IReadOnlyList<NextStep>> ListByScopeAsync(int userId, OwnerScope scope, bool? openOnly, int? take, bool includeDeleted, CancellationToken ct = default)
    {
        if (scope.IsEmpty) return Array.Empty<NextStep>();

        var q = Query(includeDeleted).Where(s => s.UserId == userId &&
            ((s.ProjectId.HasValue && scope.ProjectIds.Contains(s.ProjectId.Value)) ||
             (s.ComponentId.HasValue && scope.ComponentIds.Contains(s.ComponentId.Value)) ||
             (s.LearningTrackId.HasValue && scope.LearningTrackIds.Contains(s.LearningTrackId.Value)) ||
             (s.LearningModuleId.HasValue && scope.LearningModuleIds.Contains(s.LearningModuleId.Value))));

        if (openOnly == true) q = q.Where(s => !s.IsCompleted);

        var ordered = q.OrderByDescending(s => s.Priority).ThenBy(s => s.CreatedAt);
        return take.HasValue ? await ordered.Take(take.Value).ToListAsync(ct) : await ordered.ToListAsync(ct);
    }

    public async Task<IReadOnlyList<NextStep>> ListOpenAsync(int userId, CancellationToken ct = default)
        => await _db.NextSteps
            .Where(s => s.UserId == userId && !s.IsCompleted)
            .OrderByDescending(s => s.Priority).ThenBy(s => s.CreatedAt)
            .ToListAsync(ct);

    public Task<int> CountOpenAsync(int userId, CancellationToken ct = default)
        => _db.NextSteps.CountAsync(s => s.UserId == userId && !s.IsCompleted, ct);

    public async Task<IReadOnlyList<NextStep>> ListHighPriorityOpenAsync(int userId, int take, CancellationToken ct = default)
        => await _db.NextSteps
            .Where(s => s.UserId == userId && !s.IsCompleted && s.Priority == NextStepPriority.High)
            .OrderBy(s => s.CreatedAt)
            .Take(take)
            .ToListAsync(ct);

    public async Task AddAsync(NextStep step, CancellationToken ct = default)
        => await _db.NextSteps.AddAsync(step, ct);

    public void Update(NextStep step) => _db.NextSteps.Update(step);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);

    public async Task SoftDeleteByScopeAsync(OwnerScope scope, DateTime utcNow, CancellationToken ct = default)
    {
        if (scope.IsEmpty) return;
        await _db.NextSteps
            .Where(s => !s.IsDeleted &&
                ((s.ProjectId.HasValue && scope.ProjectIds.Contains(s.ProjectId.Value)) ||
                 (s.ComponentId.HasValue && scope.ComponentIds.Contains(s.ComponentId.Value)) ||
                 (s.LearningTrackId.HasValue && scope.LearningTrackIds.Contains(s.LearningTrackId.Value)) ||
                 (s.LearningModuleId.HasValue && scope.LearningModuleIds.Contains(s.LearningModuleId.Value))))
            .ExecuteUpdateAsync(u => u
                .SetProperty(s => s.IsDeleted, true)
                .SetProperty(s => s.DeletedAt, utcNow)
                .SetProperty(s => s.UpdatedAt, utcNow), ct);
    }
}
