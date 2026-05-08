using DevTrack.Domain.DTOs.LearningTracks;
using DevTrack.Domain.DTOs.NextSteps;
using DevTrack.Domain.DTOs.Projects;
using DevTrack.Domain.DTOs.Reminders;
using DevTrack.Domain.DTOs.Worklogs;

namespace DevTrack.Domain.DTOs.Dashboard;

public class DashboardResponse
{
    public IReadOnlyList<ProjectResponse> ActiveProjects { get; set; } = Array.Empty<ProjectResponse>();
    public IReadOnlyList<ProjectResponse> StaleProjects { get; set; } = Array.Empty<ProjectResponse>();
    public IReadOnlyList<LearningTrackResponse> ActiveLearningTracks { get; set; } = Array.Empty<LearningTrackResponse>();
    public IReadOnlyList<LearningTrackResponse> StaleLearningTracks { get; set; } = Array.Empty<LearningTrackResponse>();
    public int OpenNextStepsCount { get; set; }
    public IReadOnlyList<NextStepResponse> HighPriorityOpenNextSteps { get; set; } = Array.Empty<NextStepResponse>();
    public IReadOnlyList<WorklogResponse> RecentWorklogs { get; set; } = Array.Empty<WorklogResponse>();
    public IReadOnlyList<ReminderResponse> UnreadReminders { get; set; } = Array.Empty<ReminderResponse>();
    public int UnreadRemindersCount { get; set; }
}
