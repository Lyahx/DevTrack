using DevTrack.Domain.Enums;

namespace DevTrack.Domain.DTOs.AiImport;

public class AiExtractionResult
{
    public List<AiWorklogItem> Worklogs { get; set; } = new();
    public List<AiDecisionItem> Decisions { get; set; } = new();
    public List<AiNextStepItem> NextSteps { get; set; } = new();
    public List<AiIdeaItem> Ideas { get; set; } = new();
    public List<AiResourceItem> Resources { get; set; } = new();
}

public class AiWorklogItem
{
    public string WhatIDid { get; set; } = string.Empty;
    public string? WhatsLeft { get; set; }
}

public class AiDecisionItem
{
    public string Title { get; set; } = string.Empty;
    public string Reasoning { get; set; } = string.Empty;
    public string? Alternatives { get; set; }
}

public class AiNextStepItem
{
    public string Description { get; set; } = string.Empty;
    public NextStepPriority Priority { get; set; } = NextStepPriority.Medium;
}

public class AiIdeaItem
{
    public string Content { get; set; } = string.Empty;
}

public class AiResourceItem
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public ResourceType Type { get; set; } = ResourceType.Other;
    public string? Notes { get; set; }
}

public class AiImportRequest
{
    public string Transcript { get; set; } = string.Empty;
}

public class AiImportApplyRequest
{
    public List<AiWorklogItem> Worklogs { get; set; } = new();
    public List<AiDecisionItem> Decisions { get; set; } = new();
    public List<AiNextStepItem> NextSteps { get; set; } = new();
    public List<AiIdeaItem> Ideas { get; set; } = new();
    public List<AiResourceItem> Resources { get; set; } = new();
}

public class AiImportApplyResult
{
    public int WorklogsCreated { get; set; }
    public int DecisionsCreated { get; set; }
    public int NextStepsCreated { get; set; }
    public int IdeasCreated { get; set; }
    public int ResourcesCreated { get; set; }
}
