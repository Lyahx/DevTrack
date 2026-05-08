using AutoMapper;
using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.Reminders;
using DevTrack.Domain.Exceptions;
using DevTrack.Repository.Interfaces;
using DevTrack.Service.Interfaces;

namespace DevTrack.Service.Implementations;

public class ReminderService : IReminderService
{
    private readonly IReminderRepository _reminders;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public ReminderService(IReminderRepository reminders, ICurrentUser currentUser, IMapper mapper)
    {
        _reminders = reminders;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<PagedResult<ReminderResponse>> ListAsync(ReminderListQuery query, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize is < 1 or > 100 ? 20 : query.PageSize;
        query.Page = page; query.PageSize = pageSize;

        var (items, total) = await _reminders.ListAsync(userId, query, ct);
        return new PagedResult<ReminderResponse>
        {
            Items = items.Select(r => _mapper.Map<ReminderResponse>(r)).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<IReadOnlyList<ReminderResponse>> ListUnreadAsync(CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var items = await _reminders.ListUnreadAsync(userId, ct);
        return items.Select(r => _mapper.Map<ReminderResponse>(r)).ToList();
    }

    public async Task<ReminderResponse> MarkReadAsync(int id, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var reminder = await _reminders.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Reminder not found.");

        reminder.IsRead = true;
        await _reminders.SaveChangesAsync(ct);
        return _mapper.Map<ReminderResponse>(reminder);
    }

    public async Task<ReminderResponse> DismissAsync(int id, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var reminder = await _reminders.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Reminder not found.");

        reminder.IsDismissed = true;
        await _reminders.SaveChangesAsync(ct);
        return _mapper.Map<ReminderResponse>(reminder);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var reminder = await _reminders.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Reminder not found.");

        reminder.IsDeleted = true;
        reminder.DeletedAt = DateTime.UtcNow;
        await _reminders.SaveChangesAsync(ct);
    }
}
