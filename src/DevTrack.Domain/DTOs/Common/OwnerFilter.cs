namespace DevTrack.Domain.DTOs.Common;

public class OwnerFilter
{
    public int? ProjectId { get; set; }
    public int? ComponentId { get; set; }
    public int? LearningTrackId { get; set; }
    public int? LearningModuleId { get; set; }
}
