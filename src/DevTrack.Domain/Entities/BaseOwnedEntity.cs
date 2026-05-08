namespace DevTrack.Domain.Entities;

public abstract class BaseOwnedEntity : SoftDeletableEntity
{
    public int UserId { get; set; }
    public int? ProjectId { get; set; }
    public int? ComponentId { get; set; }
    public int? LearningTrackId { get; set; }
    public int? LearningModuleId { get; set; }

    public User User { get; set; } = null!;
    public Project? Project { get; set; }
    public Component? Component { get; set; }
    public LearningTrack? LearningTrack { get; set; }
    public LearningModule? LearningModule { get; set; }
}
