using DevTrack.Domain.DTOs.Reminders;
using DevTrack.Domain.Entities;
using DevTrack.Domain.Enums;
using DevTrack.Infrastructure.Data;
using DevTrack.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Repository.Implementations;

public class ReminderRepository : IReminderRepository
{
    private readonly DevTrackDbContext _db;

    public ReminderRepository(DevTrackDbContext db)
    {
        _db = db;
    }

    private IQueryable<Reminder> Query(bool includeDeleted)
    {
        IQueryable<Reminder> q = _db.Reminders;
        if (includeDeleted) q = q.IgnoreQueryFilters();
        return q;
    }

    public Task<Reminder?> GetByIdAsync(int id, int userId, bool includeDeleted, CancellationToken ct = default)
        => Query(includeDeleted).FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId, ct);

    public async Task<(IReadOnlyList<Reminder> Items, int Total)> ListAsync(int userId, ReminderListQuery query, CancellationToken ct = default)
    {
        var q = Query(query.IncludeDeleted).Where(r => r.UserId == userId);

        if (query.IsRead.HasValue) q = q.Where(r => r.IsRead == query.IsRead.Value);
        if (query.IsDismissed.HasValue) q = q.Where(r => r.IsDismissed == query.IsDismissed.Value);

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(r => r.GeneratedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<IReadOnlyList<Reminder>> ListUnreadAsync(int userId, CancellationToken ct = default)
        => await _db.Reminders
            .Where(r => r.UserId == userId && !r.IsRead && !r.IsDismissed)
            .OrderByDescending(r => r.GeneratedAt)
            .ToListAsync(ct);

    public Task<int> CountUnreadAsync(int userId, CancellationToken ct = default)
        => _db.Reminders.CountAsync(r => r.UserId == userId && !r.IsRead && !r.IsDismissed, ct);

    public Task<bool> ActiveProjectReminderExistsAsync(int userId, int projectId, ReminderType type, CancellationToken ct = default)
        => _db.Reminders.AnyAsync(r =>
            r.UserId == userId &&
            r.RelatedProjectId == projectId &&
            r.Type == type &&
            !r.IsRead &&
            !r.IsDismissed, ct);

    public Task<bool> ActiveLearningTrackReminderExistsAsync(int userId, int trackId, ReminderType type, CancellationToken ct = default)
        => _db.Reminders.AnyAsync(r =>
            r.UserId == userId &&
            r.RelatedLearningTrackId == trackId &&
            r.Type == type &&
            !r.IsRead &&
            !r.IsDismissed, ct);

    public async Task AddAsync(Reminder reminder, CancellationToken ct = default)
        => await _db.Reminders.AddAsync(reminder, ct);

    public void Update(Reminder reminder) => _db.Reminders.Update(reminder);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
