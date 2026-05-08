using DevTrack.Domain.Common;
using DevTrack.Domain.Enums;

namespace DevTrack.Domain.DTOs.NextSteps;

public class NextStepCreateRequest
{
    public OwnerReference Owner { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public NextStepPriority Priority { get; set; } = NextStepPriority.Medium;
}

public class NextStepUpdateRequest
{
    public string Description { get; set; } = string.Empty;
    public NextStepPriority Priority { get; set; }
}

public class NextStepResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public OwnerReference Owner { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public NextStepPriority Priority { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class NextStepListQuery
{
    public int? ProjectId { get; set; }
    public int? ComponentId { get; set; }
    public int? LearningTrackId { get; set; }
    public int? LearningModuleId { get; set; }
    public bool? IsCompleted { get; set; }
    public NextStepPriority? Priority { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public bool IncludeDeleted { get; set; } = false;
}
