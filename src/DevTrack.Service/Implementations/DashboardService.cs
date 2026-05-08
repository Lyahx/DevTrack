using AutoMapper;
using DevTrack.Domain.DTOs.Dashboard;
using DevTrack.Domain.DTOs.LearningTracks;
using DevTrack.Domain.DTOs.NextSteps;
using DevTrack.Domain.DTOs.Projects;
using DevTrack.Domain.DTOs.Reminders;
using DevTrack.Domain.DTOs.Worklogs;
using DevTrack.Repository.Interfaces;
using DevTrack.Service.Interfaces;

namespace DevTrack.Service.Implementations;

public class DashboardService : IDashboardService
{
    private const int StaleDays = 14;

    private readonly IProjectRepository _projects;
    private readonly ILearningTrackRepository _tracks;
    private readonly INextStepRepository _steps;
    private readonly IWorklogRepository _worklogs;
    private readonly IReminderRepository _reminders;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public DashboardService(
        IProjectRepository projects,
        ILearningTrackRepository tracks,
        INextStepRepository steps,
        IWorklogRepository worklogs,
        IReminderRepository reminders,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _projects = projects;
        _tracks = tracks;
        _steps = steps;
        _worklogs = worklogs;
        _reminders = reminders;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<DashboardResponse> GetAsync(CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var staleCutoff = DateTime.UtcNow.AddDays(-StaleDays);

        // Sequential awaits — DbContext is not thread-safe so Task.WhenAll over the same scoped
        // context throws "A second operation was started on this context instance".
        var activeProjectsList = await _projects.ListActiveForUserAsync(userId, ct);
        var activeTracksList = await _tracks.ListActiveForUserAsync(userId, ct);
        var openCount = await _steps.CountOpenAsync(userId, ct);
        var highPriority = await _steps.ListHighPriorityOpenAsync(userId, take: 5, ct);
        var recentWorklogsList = await _worklogs.ListRecentAsync(userId, days: 30, ct);
        var unreadRemindersList = await _reminders.ListUnreadAsync(userId, ct);
        var unreadCount = await _reminders.CountUnreadAsync(userId, ct);

        var activeProjects = activeProjectsList
            .OrderByDescending(p => p.LastActivityAt ?? p.CreatedAt)
            .Take(10)
            .Select(p => _mapper.Map<ProjectResponse>(p))
            .ToList();

        var staleProjects = activeProjectsList
            .Where(p => IsStale(p.LastActivityAt, p.CreatedAt, staleCutoff))
            .OrderBy(p => p.LastActivityAt ?? p.CreatedAt)
            .Take(10)
            .Select(p => _mapper.Map<ProjectResponse>(p))
            .ToList();

        var activeTracks = activeTracksList
            .OrderByDescending(t => t.LastActivityAt ?? t.CreatedAt)
            .Take(10)
            .Select(t => _mapper.Map<LearningTrackResponse>(t))
            .ToList();

        var staleTracks = activeTracksList
            .Where(t => IsStale(t.LastActivityAt, t.CreatedAt, staleCutoff))
            .OrderBy(t => t.LastActivityAt ?? t.CreatedAt)
            .Take(10)
            .Select(t => _mapper.Map<LearningTrackResponse>(t))
            .ToList();

        var recentWorklogs = recentWorklogsList
            .Take(10)
            .Select(w => _mapper.Map<WorklogResponse>(w))
            .ToList();

        return new DashboardResponse
        {
            ActiveProjects = activeProjects,
            StaleProjects = staleProjects,
            ActiveLearningTracks = activeTracks,
            StaleLearningTracks = staleTracks,
            OpenNextStepsCount = openCount,
            HighPriorityOpenNextSteps = highPriority.Select(s => _mapper.Map<NextStepResponse>(s)).ToList(),
            RecentWorklogs = recentWorklogs,
            UnreadReminders = unreadRemindersList.Select(r => _mapper.Map<ReminderResponse>(r)).ToList(),
            UnreadRemindersCount = unreadCount
        };
    }

    private static bool IsStale(DateTime? lastActivity, DateTime createdAt, DateTime cutoff)
        => lastActivity.HasValue ? lastActivity.Value < cutoff : createdAt < cutoff;
}
