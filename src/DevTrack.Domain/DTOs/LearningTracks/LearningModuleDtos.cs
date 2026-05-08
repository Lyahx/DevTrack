using DevTrack.Domain.Enums;

namespace DevTrack.Domain.DTOs.LearningTracks;

public class LearningModuleCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public LearningModuleStatus Status { get; set; } = LearningModuleStatus.NotStarted;
}

public class LearningModuleUpdateRequest
{
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
}

public class LearningModuleStatusUpdateRequest
{
    public LearningModuleStatus Status { get; set; }
}

public class LearningModuleOrderUpdateRequest
{
    public int Order { get; set; }
}

public class LearningModuleResponse
{
    public int Id { get; set; }
    public int LearningTrackId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public LearningModuleStatus Status { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? LastActivityAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
