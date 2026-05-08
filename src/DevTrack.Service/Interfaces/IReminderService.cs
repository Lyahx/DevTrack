using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.Reminders;

namespace DevTrack.Service.Interfaces;

public interface IReminderService
{
    Task<PagedResult<ReminderResponse>> ListAsync(ReminderListQuery query, CancellationToken ct = default);
    Task<IReadOnlyList<ReminderResponse>> ListUnreadAsync(CancellationToken ct = default);
    Task<ReminderResponse> MarkReadAsync(int id, CancellationToken ct = default);
    Task<ReminderResponse> DismissAsync(int id, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
