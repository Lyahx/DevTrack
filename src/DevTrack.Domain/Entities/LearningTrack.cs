using DevTrack.Domain.Enums;

namespace DevTrack.Domain.Entities;

public class LearningTrack : SoftDeletableEntity
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Source { get; set; }
    public LearningTrackStatus Status { get; set; } = LearningTrackStatus.Active;
    public DateTime? LastActivityAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public User User { get; set; } = null!;
    public ICollection<LearningModule> Modules { get; set; } = new List<LearningModule>();
    public ICollection<LearningTrackTag> LearningTrackTags { get; set; } = new List<LearningTrackTag>();
}
