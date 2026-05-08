using DevTrack.Domain.Entities;
using DevTrack.Infrastructure.Data;
using DevTrack.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Repository.Implementations;

public class LearningModuleRepository : ILearningModuleRepository
{
    private readonly DevTrackDbContext _db;

    public LearningModuleRepository(DevTrackDbContext db)
    {
        _db = db;
    }

    private IQueryable<LearningModule> Query(bool includeDeleted)
    {
        IQueryable<LearningModule> q = _db.LearningModules.Include(m => m.LearningTrack);
        if (includeDeleted) q = q.IgnoreQueryFilters();
        return q;
    }

    public Task<LearningModule?> GetByIdAsync(int id, int userId, bool includeDeleted, CancellationToken ct = default)
        => Query(includeDeleted).FirstOrDefaultAsync(m => m.Id == id && m.LearningTrack.UserId == userId, ct);

    public async Task<IReadOnlyList<LearningModule>> ListByTrackAsync(int trackId, int userId, bool includeDeleted, CancellationToken ct = default)
    {
        return await Query(includeDeleted)
            .Where(m => m.LearningTrackId == trackId && m.LearningTrack.UserId == userId)
            .OrderBy(m => m.Order)
            .ToListAsync(ct);
    }

    public async Task AddAsync(LearningModule module, CancellationToken ct = default)
        => await _db.LearningModules.AddAsync(module, ct);

    public void Update(LearningModule module) => _db.LearningModules.Update(module);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);

    public async Task UpdateLastActivityAsync(int id, DateTime utcNow, CancellationToken ct = default)
    {
        await _db.LearningModules.Where(m => m.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(m => m.LastActivityAt, utcNow), ct);
    }

    public async Task<int?> GetParentTrackIdAsync(int moduleId, int userId, CancellationToken ct = default)
    {
        return await _db.LearningModules
            .Where(m => m.Id == moduleId && m.LearningTrack.UserId == userId)
            .Select(m => (int?)m.LearningTrackId)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<int>> GetModuleIdsForTrackAsync(int trackId, int userId, bool includeDeleted, CancellationToken ct = default)
    {
        IQueryable<LearningModule> q = _db.LearningModules
            .Where(m => m.LearningTrackId == trackId && m.LearningTrack.UserId == userId);
        if (includeDeleted)
            q = _db.LearningModules.IgnoreQueryFilters()
                .Where(m => m.LearningTrackId == trackId && m.LearningTrack.UserId == userId);
        return await q.Select(m => m.Id).ToListAsync(ct);
    }

    public async Task<(int Total, int Completed)> GetModuleProgressAsync(int trackId, int userId, CancellationToken ct = default)
    {
        var modules = await _db.LearningModules
            .Where(m => m.LearningTrackId == trackId && m.LearningTrack.UserId == userId)
            .Select(m => m.Status)
            .ToListAsync(ct);
        return (modules.Count, modules.Count(s => s == Domain.Enums.LearningModuleStatus.Completed));
    }

    public async Task SoftDeleteByTrackAsync(int trackId, DateTime utcNow, CancellationToken ct = default)
    {
        await _db.LearningModules
            .Where(m => m.LearningTrackId == trackId && !m.IsDeleted)
            .ExecuteUpdateAsync(s => s
                .SetProperty(m => m.IsDeleted, true)
                .SetProperty(m => m.DeletedAt, utcNow)
                .SetProperty(m => m.UpdatedAt, utcNow), ct);
    }
}
