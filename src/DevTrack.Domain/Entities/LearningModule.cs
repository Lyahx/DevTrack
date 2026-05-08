using DevTrack.Domain.Enums;

namespace DevTrack.Domain.Entities;

public class LearningModule : SoftDeletableEntity
{
    public int LearningTrackId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public LearningModuleStatus Status { get; set; } = LearningModuleStatus.NotStarted;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? LastActivityAt { get; set; }

    public LearningTrack LearningTrack { get; set; } = null!;
}
