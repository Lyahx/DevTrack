namespace DevTrack.Domain.Entities;

public class Tag : SoftDeletableEntity
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }

    public User User { get; set; } = null!;
    public ICollection<ProjectTag> ProjectTags { get; set; } = new List<ProjectTag>();
    public ICollection<ComponentTag> ComponentTags { get; set; } = new List<ComponentTag>();
    public ICollection<LearningTrackTag> LearningTrackTags { get; set; } = new List<LearningTrackTag>();
}
