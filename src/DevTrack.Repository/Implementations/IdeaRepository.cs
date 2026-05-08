using DevTrack.Domain.DTOs.Ideas;
using DevTrack.Domain.Entities;
using DevTrack.Infrastructure.Data;
using DevTrack.Repository.Common;
using DevTrack.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Repository.Implementations;

public class IdeaRepository : IIdeaRepository
{
    private readonly DevTrackDbContext _db;

    public IdeaRepository(DevTrackDbContext db)
    {
        _db = db;
    }

    private IQueryable<Idea> Query(bool includeDeleted)
    {
        IQueryable<Idea> q = _db.Ideas;
        if (includeDeleted) q = q.IgnoreQueryFilters();
        return q;
    }

    public Task<Idea?> GetByIdAsync(int id, int userId, bool includeDeleted, CancellationToken ct = default)
        => Query(includeDeleted).FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId, ct);

    public async Task<(IReadOnlyList<Idea> Items, int Total)> ListAsync(int userId, IdeaListQuery query, CancellationToken ct = default)
    {
        var q = Query(query.IncludeDeleted).Where(i => i.UserId == userId);

        if (query.ProjectId.HasValue) q = q.Where(i => i.ProjectId == query.ProjectId);
        if (query.ComponentId.HasValue) q = q.Where(i => i.ComponentId == query.ComponentId);
        if (query.LearningTrackId.HasValue) q = q.Where(i => i.LearningTrackId == query.LearningTrackId);
        if (query.LearningModuleId.HasValue) q = q.Where(i => i.LearningModuleId == query.LearningModuleId);
        if (query.IsConvertedToNextStep.HasValue) q = q.Where(i => i.IsConvertedToNextStep == query.IsConvertedToNextStep.Value);

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(i => i.CapturedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<IReadOnlyList<Idea>> ListByScopeAsync(int userId, OwnerScope scope, bool? unconvertedOnly, int? take, bool includeDeleted, CancellationToken ct = default)
    {
        if (scope.IsEmpty) return Array.Empty<Idea>();

        var q = Query(includeDeleted).Where(i => i.UserId == userId &&
            ((i.ProjectId.HasValue && scope.ProjectIds.Contains(i.ProjectId.Value)) ||
             (i.ComponentId.HasValue && scope.ComponentIds.Contains(i.ComponentId.Value)) ||
             (i.LearningTrackId.HasValue && scope.LearningTrackIds.Contains(i.LearningTrackId.Value)) ||
             (i.LearningModuleId.HasValue && scope.LearningModuleIds.Contains(i.LearningModuleId.Value))));

        if (unconvertedOnly == true) q = q.Where(i => !i.IsConvertedToNextStep);

        var ordered = q.OrderByDescending(i => i.CapturedAt);
        return take.HasValue ? await ordered.Take(take.Value).ToListAsync(ct) : await ordered.ToListAsync(ct);
    }

    public async Task AddAsync(Idea idea, CancellationToken ct = default)
        => await _db.Ideas.AddAsync(idea, ct);

    public void Update(Idea idea) => _db.Ideas.Update(idea);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);

    public async Task SoftDeleteByScopeAsync(OwnerScope scope, DateTime utcNow, CancellationToken ct = default)
    {
        if (scope.IsEmpty) return;
        await _db.Ideas
            .Where(i => !i.IsDeleted &&
                ((i.ProjectId.HasValue && scope.ProjectIds.Contains(i.ProjectId.Value)) ||
                 (i.ComponentId.HasValue && scope.ComponentIds.Contains(i.ComponentId.Value)) ||
                 (i.LearningTrackId.HasValue && scope.LearningTrackIds.Contains(i.LearningTrackId.Value)) ||
                 (i.LearningModuleId.HasValue && scope.LearningModuleIds.Contains(i.LearningModuleId.Value))))
            .ExecuteUpdateAsync(s => s
                .SetProperty(i => i.IsDeleted, true)
                .SetProperty(i => i.DeletedAt, utcNow)
                .SetProperty(i => i.UpdatedAt, utcNow), ct);
    }
}
