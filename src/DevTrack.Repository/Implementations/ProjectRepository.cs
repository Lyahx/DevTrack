using DevTrack.Domain.Entities;
using DevTrack.Domain.Enums;
using DevTrack.Infrastructure.Data;
using DevTrack.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Repository.Implementations;

public class ProjectRepository : IProjectRepository
{
    private readonly DevTrackDbContext _db;

    public ProjectRepository(DevTrackDbContext db)
    {
        _db = db;
    }

    private IQueryable<Project> Query(bool includeDeleted)
    {
        IQueryable<Project> q = _db.Projects
            .Include(p => p.ProjectTags).ThenInclude(pt => pt.Tag);
        if (includeDeleted) q = q.IgnoreQueryFilters();
        return q;
    }

    public Task<Project?> GetByIdAsync(int id, int userId, bool includeDeleted, CancellationToken ct = default)
        => Query(includeDeleted).FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId, ct);

    public async Task<(IReadOnlyList<Project> Items, int Total)> ListAsync(
        int userId,
        ProjectStatus? status,
        int? tagId,
        int page,
        int pageSize,
        bool includeDeleted,
        CancellationToken ct = default)
    {
        var q = Query(includeDeleted).Where(p => p.UserId == userId);
        if (status.HasValue) q = q.Where(p => p.Status == status.Value);
        if (tagId.HasValue) q = q.Where(p => p.ProjectTags.Any(pt => pt.TagId == tagId.Value));

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(p => p.LastActivityAt ?? p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task AddAsync(Project project, CancellationToken ct = default)
        => await _db.Projects.AddAsync(project, ct);

    public void Update(Project project) => _db.Projects.Update(project);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);

    public async Task UpdateLastActivityAsync(int id, DateTime utcNow, CancellationToken ct = default)
    {
        await _db.Projects.Where(p => p.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.LastActivityAt, utcNow), ct);
    }

    public async Task<IReadOnlyList<Project>> ListActiveForUserAsync(int userId, CancellationToken ct = default)
        => await _db.Projects
            .Where(p => p.UserId == userId && p.Status == ProjectStatus.Active)
            .ToListAsync(ct);
}
