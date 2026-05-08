namespace DevTrack.Domain.Entities;

public class User : SoftDeletableEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<LearningTrack> LearningTracks { get; set; } = new List<LearningTrack>();
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
