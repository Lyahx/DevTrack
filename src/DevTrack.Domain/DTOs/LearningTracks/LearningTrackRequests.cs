using DevTrack.Domain.Enums;

namespace DevTrack.Domain.DTOs.LearningTracks;

public class LearningTrackCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Source { get; set; }
    public LearningTrackStatus Status { get; set; } = LearningTrackStatus.Active;
}

public class LearningTrackUpdateRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Source { get; set; }
}

public class LearningTrackStatusUpdateRequest
{
    public LearningTrackStatus Status { get; set; }
}

public class LearningTrackListQuery
{
    public LearningTrackStatus? Status { get; set; }
    public int? TagId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public bool IncludeDeleted { get; set; } = false;
}
