using DevTrack.Domain.Enums;

namespace DevTrack.Domain.DTOs.Reminders;

public class ReminderResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? RelatedProjectId { get; set; }
    public int? RelatedLearningTrackId { get; set; }
    public ReminderType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public ReminderSeverity Severity { get; set; }
    public bool IsRead { get; set; }
    public bool IsDismissed { get; set; }
    public DateTime GeneratedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class ReminderListQuery
{
    public bool? IsRead { get; set; }
    public bool? IsDismissed { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public bool IncludeDeleted { get; set; } = false;
}
