using DevTrack.Domain.DTOs.Reminders;
using DevTrack.Domain.Entities;
using DevTrack.Domain.Enums;

namespace DevTrack.Repository.Interfaces;

public interface IReminderRepository
{
    Task<Reminder?> GetByIdAsync(int id, int userId, bool includeDeleted, CancellationToken ct = default);
    Task<(IReadOnlyList<Reminder> Items, int Total)> ListAsync(int userId, ReminderListQuery query, CancellationToken ct = default);
    Task<IReadOnlyList<Reminder>> ListUnreadAsync(int userId, CancellationToken ct = default);
    Task<int> CountUnreadAsync(int userId, CancellationToken ct = default);
    Task<bool> ActiveProjectReminderExistsAsync(int userId, int projectId, ReminderType type, CancellationToken ct = default);
    Task<bool> ActiveLearningTrackReminderExistsAsync(int userId, int trackId, ReminderType type, CancellationToken ct = default);
    Task AddAsync(Reminder reminder, CancellationToken ct = default);
    void Update(Reminder reminder);
    Task SaveChangesAsync(CancellationToken ct = default);
}
