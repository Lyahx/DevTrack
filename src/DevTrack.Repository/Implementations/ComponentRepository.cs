using DevTrack.Domain.Entities;
using DevTrack.Infrastructure.Data;
using DevTrack.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Repository.Implementations;

public class ComponentRepository : IComponentRepository
{
    private readonly DevTrackDbContext _db;

    public ComponentRepository(DevTrackDbContext db)
    {
        _db = db;
    }

    private IQueryable<Component> Query(bool includeDeleted)
    {
        IQueryable<Component> q = _db.Components
            .Include(c => c.Project)
            .Include(c => c.ComponentTags).ThenInclude(ct => ct.Tag);
        if (includeDeleted) q = q.IgnoreQueryFilters();
        return q;
    }

    public Task<Component?> GetByIdAsync(int id, int userId, bool includeDeleted, CancellationToken ct = default)
        => Query(includeDeleted).FirstOrDefaultAsync(c => c.Id == id && c.Project.UserId == userId, ct);

    public async Task<IReadOnlyList<Component>> ListByProjectAsync(int projectId, int userId, bool includeDeleted, CancellationToken ct = default)
    {
        return await Query(includeDeleted)
            .Where(c => c.ProjectId == projectId && c.Project.UserId == userId)
            .OrderByDescending(c => c.LastActivityAt ?? c.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Component component, CancellationToken ct = default)
        => await _db.Components.AddAsync(component, ct);

    public void Update(Component component) => _db.Components.Update(component);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);

    public async Task UpdateLastActivityAsync(int id, DateTime utcNow, CancellationToken ct = default)
    {
        await _db.Components.Where(c => c.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.LastActivityAt, utcNow), ct);
    }

    public async Task<int?> GetParentProjectIdAsync(int componentId, int userId, CancellationToken ct = default)
    {
        return await _db.Components
            .Where(c => c.Id == componentId && c.Project.UserId == userId)
            .Select(c => (int?)c.ProjectId)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<int>> GetComponentIdsForProjectAsync(int projectId, int userId, bool includeDeleted, CancellationToken ct = default)
    {
        IQueryable<Component> q = _db.Components.Where(c => c.ProjectId == projectId && c.Project.UserId == userId);
        if (includeDeleted) q = q.IgnoreQueryFilters().Where(c => c.ProjectId == projectId && c.Project.UserId == userId);
        return await q.Select(c => c.Id).ToListAsync(ct);
    }

    public async Task SoftDeleteByProjectAsync(int projectId, DateTime utcNow, CancellationToken ct = default)
    {
        await _db.Components
            .Where(c => c.ProjectId == projectId && !c.IsDeleted)
            .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.IsDeleted, true)
                .SetProperty(c => c.DeletedAt, utcNow)
                .SetProperty(c => c.UpdatedAt, utcNow), ct);
    }
}
