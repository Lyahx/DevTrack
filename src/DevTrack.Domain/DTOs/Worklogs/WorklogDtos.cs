using DevTrack.Domain.Common;

namespace DevTrack.Domain.DTOs.Worklogs;

public class WorklogCreateRequest
{
    public OwnerReference Owner { get; set; } = null!;
    public string WhatIDid { get; set; } = string.Empty;
    public string? WhatsLeft { get; set; }
    public string? Reasoning { get; set; }
    public string? Alternatives { get; set; }
    public DateTime? LoggedAt { get; set; }
}

public class WorklogUpdateRequest
{
    public string WhatIDid { get; set; } = string.Empty;
    public string? WhatsLeft { get; set; }
    public string? Reasoning { get; set; }
    public string? Alternatives { get; set; }
    public DateTime? LoggedAt { get; set; }
}

public class WorklogResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public OwnerReference Owner { get; set; } = null!;
    public string WhatIDid { get; set; } = string.Empty;
    public string? WhatsLeft { get; set; }
    public string? Reasoning { get; set; }
    public string? Alternatives { get; set; }
    public DateTime LoggedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class WorklogListQuery
{
    public int? ProjectId { get; set; }
    public int? ComponentId { get; set; }
    public int? LearningTrackId { get; set; }
    public int? LearningModuleId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public bool IncludeDeleted { get; set; } = false;
}
