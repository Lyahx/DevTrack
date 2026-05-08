using DevTrack.Domain.Enums;

namespace DevTrack.Domain.Entities;

public class Reminder : SoftDeletableEntity
{
    public int UserId { get; set; }
    public int? RelatedProjectId { get; set; }
    public int? RelatedLearningTrackId { get; set; }
    public ReminderType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public ReminderSeverity Severity { get; set; } = ReminderSeverity.Info;
    public bool IsRead { get; set; }
    public bool IsDismissed { get; set; }
    public DateTime GeneratedAt { get; set; }

    public User User { get; set; } = null!;
    public Project? RelatedProject { get; set; }
    public LearningTrack? RelatedLearningTrack { get; set; }
}
