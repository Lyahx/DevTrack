using DevTrack.Domain.DTOs.Resources;
using DevTrack.Domain.Entities;
using DevTrack.Infrastructure.Data;
using DevTrack.Repository.Common;
using DevTrack.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Repository.Implementations;

public class ResourceRepository : IResourceRepository
{
    private readonly DevTrackDbContext _db;

    public ResourceRepository(DevTrackDbContext db)
    {
        _db = db;
    }

    private IQueryable<Resource> Query(bool includeDeleted)
    {
        IQueryable<Resource> q = _db.Resources;
        if (includeDeleted) q = q.IgnoreQueryFilters();
        return q;
    }

    public Task<Resource?> GetByIdAsync(int id, int userId, bool includeDeleted, CancellationToken ct = default)
        => Query(includeDeleted).FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId, ct);

    public async Task<(IReadOnlyList<Resource> Items, int Total)> ListAsync(int userId, ResourceListQuery query, CancellationToken ct = default)
    {
        var q = Query(query.IncludeDeleted).Where(r => r.UserId == userId);

        if (query.ProjectId.HasValue) q = q.Where(r => r.ProjectId == query.ProjectId);
        if (query.ComponentId.HasValue) q = q.Where(r => r.ComponentId == query.ComponentId);
        if (query.LearningTrackId.HasValue) q = q.Where(r => r.LearningTrackId == query.LearningTrackId);
        if (query.LearningModuleId.HasValue) q = q.Where(r => r.LearningModuleId == query.LearningModuleId);
        if (query.Type.HasValue) q = q.Where(r => r.Type == query.Type.Value);

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(r => r.AddedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<IReadOnlyList<Resource>> ListByScopeAsync(int userId, OwnerScope scope, bool includeDeleted, CancellationToken ct = default)
    {
        if (scope.IsEmpty) return Array.Empty<Resource>();

        return await Query(includeDeleted)
            .Where(r => r.UserId == userId &&
                ((r.ProjectId.HasValue && scope.ProjectIds.Contains(r.ProjectId.Value)) ||
                 (r.ComponentId.HasValue && scope.ComponentIds.Contains(r.ComponentId.Value)) ||
                 (r.LearningTrackId.HasValue && scope.LearningTrackIds.Contains(r.LearningTrackId.Value)) ||
                 (r.LearningModuleId.HasValue && scope.LearningModuleIds.Contains(r.LearningModuleId.Value))))
            .OrderByDescending(r => r.AddedAt)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Resource resource, CancellationToken ct = default)
        => await _db.Resources.AddAsync(resource, ct);

    public void Update(Resource resource) => _db.Resources.Update(resource);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);

    public async Task SoftDeleteByScopeAsync(OwnerScope scope, DateTime utcNow, CancellationToken ct = default)
    {
        if (scope.IsEmpty) return;
        await _db.Resources
            .Where(r => !r.IsDeleted &&
                ((r.ProjectId.HasValue && scope.ProjectIds.Contains(r.ProjectId.Value)) ||
                 (r.ComponentId.HasValue && scope.ComponentIds.Contains(r.ComponentId.Value)) ||
                 (r.LearningTrackId.HasValue && scope.LearningTrackIds.Contains(r.LearningTrackId.Value)) ||
                 (r.LearningModuleId.HasValue && scope.LearningModuleIds.Contains(r.LearningModuleId.Value))))
            .ExecuteUpdateAsync(s => s
                .SetProperty(r => r.IsDeleted, true)
                .SetProperty(r => r.DeletedAt, utcNow)
                .SetProperty(r => r.UpdatedAt, utcNow), ct);
    }
}
