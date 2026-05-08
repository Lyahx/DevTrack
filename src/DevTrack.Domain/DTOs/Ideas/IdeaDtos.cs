using DevTrack.Domain.Common;
using DevTrack.Domain.Enums;

namespace DevTrack.Domain.DTOs.Ideas;

public class IdeaCreateRequest
{
    public OwnerReference Owner { get; set; } = null!;
    public string Content { get; set; } = string.Empty;
}

public class IdeaUpdateRequest
{
    public string Content { get; set; } = string.Empty;
}

public class IdeaResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public OwnerReference Owner { get; set; } = null!;
    public string Content { get; set; } = string.Empty;
    public bool IsConvertedToNextStep { get; set; }
    public int? ConvertedNextStepId { get; set; }
    public DateTime CapturedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class IdeaListQuery
{
    public int? ProjectId { get; set; }
    public int? ComponentId { get; set; }
    public int? LearningTrackId { get; set; }
    public int? LearningModuleId { get; set; }
    public bool? IsConvertedToNextStep { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public bool IncludeDeleted { get; set; } = false;
}

public class IdeaConvertRequest
{
    public NextStepPriority Priority { get; set; } = NextStepPriority.Medium;
}
