using DevTrack.Domain.DTOs.Components;
using DevTrack.Domain.DTOs.Ideas;
using DevTrack.Domain.DTOs.LearningTracks;
using DevTrack.Domain.DTOs.NextSteps;
using DevTrack.Domain.DTOs.Projects;
using DevTrack.Domain.DTOs.Resources;
using DevTrack.Domain.DTOs.Worklogs;

namespace DevTrack.Domain.DTOs.Resume;

public class ResumeProjectResponse
{
    public ProjectResponse Project { get; set; } = null!;
    public IReadOnlyList<ComponentResponse> Components { get; set; } = Array.Empty<ComponentResponse>();
    public IReadOnlyList<WorklogResponse> RecentWorklogs { get; set; } = Array.Empty<WorklogResponse>();
    public IReadOnlyList<NextStepResponse> OpenNextSteps { get; set; } = Array.Empty<NextStepResponse>();
    public IReadOnlyList<ResourceResponse> Resources { get; set; } = Array.Empty<ResourceResponse>();
    public IReadOnlyList<IdeaResponse> RecentIdeas { get; set; } = Array.Empty<IdeaResponse>();
    public int? DaysSinceLastActivity { get; set; }
}

public class ResumeComponentResponse
{
    public ComponentResponse Component { get; set; } = null!;
    public IReadOnlyList<WorklogResponse> RecentWorklogs { get; set; } = Array.Empty<WorklogResponse>();
    public IReadOnlyList<NextStepResponse> OpenNextSteps { get; set; } = Array.Empty<NextStepResponse>();
    public IReadOnlyList<ResourceResponse> Resources { get; set; } = Array.Empty<ResourceResponse>();
    public IReadOnlyList<IdeaResponse> RecentIdeas { get; set; } = Array.Empty<IdeaResponse>();
    public int? DaysSinceLastActivity { get; set; }
}

public class ResumeLearningTrackResponse
{
    public LearningTrackResponse Track { get; set; } = null!;
    public IReadOnlyList<LearningModuleResponse> Modules { get; set; } = Array.Empty<LearningModuleResponse>();
    public IReadOnlyList<WorklogResponse> RecentWorklogs { get; set; } = Array.Empty<WorklogResponse>();
    public IReadOnlyList<NextStepResponse> OpenNextSteps { get; set; } = Array.Empty<NextStepResponse>();
    public IReadOnlyList<ResourceResponse> Resources { get; set; } = Array.Empty<ResourceResponse>();
    public IReadOnlyList<IdeaResponse> RecentIdeas { get; set; } = Array.Empty<IdeaResponse>();
    public decimal ProgressPercent { get; set; }
    public int? DaysSinceLastActivity { get; set; }
}

public class ResumeLearningModuleResponse
{
    public LearningModuleResponse Module { get; set; } = null!;
    public IReadOnlyList<WorklogResponse> RecentWorklogs { get; set; } = Array.Empty<WorklogResponse>();
    public IReadOnlyList<NextStepResponse> OpenNextSteps { get; set; } = Array.Empty<NextStepResponse>();
    public IReadOnlyList<ResourceResponse> Resources { get; set; } = Array.Empty<ResourceResponse>();
    public IReadOnlyList<IdeaResponse> RecentIdeas { get; set; } = Array.Empty<IdeaResponse>();
    public int? DaysSinceLastActivity { get; set; }
}
