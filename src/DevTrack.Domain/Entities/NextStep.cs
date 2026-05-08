using DevTrack.Domain.Enums;

namespace DevTrack.Domain.Entities;

public class NextStep : BaseOwnedEntity
{
    public string Description { get; set; } = string.Empty;
    public NextStepPriority Priority { get; set; } = NextStepPriority.Medium;
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
}
