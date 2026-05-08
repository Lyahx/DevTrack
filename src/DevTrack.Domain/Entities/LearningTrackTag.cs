namespace DevTrack.Domain.Entities;

public class LearningTrackTag
{
    public int LearningTrackId { get; set; }
    public int TagId { get; set; }

    public LearningTrack LearningTrack { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}
