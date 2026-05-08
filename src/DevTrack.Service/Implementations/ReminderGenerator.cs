using DevTrack.Domain.Entities;
using DevTrack.Domain.Enums;
using DevTrack.Repository.Interfaces;
using DevTrack.Service.Interfaces;
using Microsoft.Extensions.Logging;

namespace DevTrack.Service.Implementations;

public class ReminderGenerator : IReminderGenerator
{
    private const int InactivityThresholdDays = 14;

    private readonly IUserRepository _users;
    private readonly IProjectRepository _projects;
    private readonly ILearningTrackRepository _tracks;
    private readonly IReminderRepository _reminders;
    private readonly ILogger<ReminderGenerator> _logger;

    public ReminderGenerator(
        IUserRepository users,
        IProjectRepository projects,
        ILearningTrackRepository tracks,
        IReminderRepository reminders,
        ILogger<ReminderGenerator> logger)
    {
        _users = users;
        _projects = projects;
        _tracks = tracks;
        _reminders = reminders;
        _logger = logger;
    }

    public async Task<int> GenerateForAllUsersAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var threshold = now.AddDays(-InactivityThresholdDays);
        var generated = 0;

        var users = await _users.ListAllAsync(ct);
        foreach (var user in users)
        {
            generated += await GenerateForProjectsAsync(user, threshold, now, ct);
            generated += await GenerateForLearningTracksAsync(user, threshold, now, ct);
        }

        if (generated > 0)
        {
            await _reminders.SaveChangesAsync(ct);
            _logger.LogInformation("Reminder generator created {Count} reminder(s).", generated);
        }
        else
        {
            _logger.LogInformation("Reminder generator ran; no new reminders created.");
        }

        return generated;
    }

    private async Task<int> GenerateForProjectsAsync(User user, DateTime threshold, DateTime now, CancellationToken ct)
    {
        var created = 0;
        var projects = await _projects.ListActiveForUserAsync(user.Id, ct);
        foreach (var project in projects)
        {
            var lastTouch = project.LastActivityAt ?? project.CreatedAt;
            if (lastTouch >= threshold) continue;

            if (await _reminders.ActiveProjectReminderExistsAsync(user.Id, project.Id, ReminderType.ProjectInactive, ct))
                continue;

            var days = (int)Math.Floor((now - lastTouch).TotalDays);
            await _reminders.AddAsync(new Reminder
            {
                UserId = user.Id,
                RelatedProjectId = project.Id,
                Type = ReminderType.ProjectInactive,
                Severity = ReminderSeverity.Warning,
                Title = $"{project.Name} duruyor",
                Message = $"{project.Name} projesine {days} gündür dokunmadın. Hala aktif mi?",
                GeneratedAt = now
            }, ct);
            created++;
        }
        return created;
    }

    private async Task<int> GenerateForLearningTracksAsync(User user, DateTime threshold, DateTime now, CancellationToken ct)
    {
        var created = 0;
        var tracks = await _tracks.ListActiveForUserAsync(user.Id, ct);
        foreach (var track in tracks)
        {
            var lastTouch = track.LastActivityAt ?? track.CreatedAt;
            if (lastTouch >= threshold) continue;

            if (await _reminders.ActiveLearningTrackReminderExistsAsync(user.Id, track.Id, ReminderType.LearningTrackInactive, ct))
                continue;

            var days = (int)Math.Floor((now - lastTouch).TotalDays);
            await _reminders.AddAsync(new Reminder
            {
                UserId = user.Id,
                RelatedLearningTrackId = track.Id,
                Type = ReminderType.LearningTrackInactive,
                Severity = ReminderSeverity.Warning,
                Title = $"{track.Name} duruyor",
                Message = $"{track.Name} eğitiminde {days} gündür ilerleme yok. Hala devam ediyor musun?",
                GeneratedAt = now
            }, ct);
            created++;
        }
        return created;
    }
}
