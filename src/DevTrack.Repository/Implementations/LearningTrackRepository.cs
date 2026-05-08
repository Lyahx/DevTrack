using DevTrack.Domain.Entities;
using DevTrack.Domain.Enums;
using DevTrack.Infrastructure.Data;
using DevTrack.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Repository.Implementations;

public class LearningTrackRepository : ILearningTrackRepository
{
    private readonly DevTrackDbContext _db;

    public LearningTrackRepository(DevTrackDbContext db)
    {
        _db = db;
    }

    private IQueryable<LearningTrack> Query(bool includeDeleted)
    {
        IQueryable<LearningTrack> q = _db.LearningTracks
            .Include(t => t.LearningTrackTags).ThenInclude(lt => lt.Tag);
        if (includeDeleted) q = q.IgnoreQueryFilters();
        return q;
    }

    public Task<LearningTrack?> GetByIdAsync(int id, int userId, bool includeDeleted, CancellationToken ct = default)
        => Query(includeDeleted).FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, ct);

    public async Task<(IReadOnlyList<LearningTrack> Items, int Total)> ListAsync(
        int userId,
        LearningTrackStatus? status,
        int? tagId,
        int page,
        int pageSize,
        bool includeDeleted,
        CancellationToken ct = default)
    {
        var q = Query(includeDeleted).Where(t => t.UserId == userId);
        if (status.HasValue) q = q.Where(t => t.Status == status.Value);
        if (tagId.HasValue) q = q.Where(t => t.LearningTrackTags.Any(x => x.TagId == tagId.Value));

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(t => t.LastActivityAt ?? t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task AddAsync(LearningTrack track, CancellationToken ct = default)
        => await _db.LearningTracks.AddAsync(track, ct);

    public void Update(LearningTrack track) => _db.LearningTracks.Update(track);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);

    public async Task UpdateLastActivityAsync(int id, DateTime utcNow, CancellationToken ct = default)
    {
        await _db.LearningTracks.Where(t => t.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.LastActivityAt, utcNow), ct);
    }

    public async Task<IReadOnlyList<LearningTrack>> ListActiveForUserAsync(int userId, CancellationToken ct = default)
        => await _db.LearningTracks
            .Where(t => t.UserId == userId && t.Status == LearningTrackStatus.Active)
            .ToListAsync(ct);
}
