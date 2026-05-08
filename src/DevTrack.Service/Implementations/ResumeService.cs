using AutoMapper;
using DevTrack.Domain.DTOs.Components;
using DevTrack.Domain.DTOs.Decisions;
using DevTrack.Domain.DTOs.Ideas;
using DevTrack.Domain.DTOs.LearningTracks;
using DevTrack.Domain.DTOs.NextSteps;
using DevTrack.Domain.DTOs.Projects;
using DevTrack.Domain.DTOs.Resources;
using DevTrack.Domain.DTOs.Resume;
using DevTrack.Domain.DTOs.Worklogs;
using DevTrack.Domain.Exceptions;
using DevTrack.Repository.Common;
using DevTrack.Repository.Interfaces;
using DevTrack.Service.Interfaces;

namespace DevTrack.Service.Implementations;

public class ResumeService : IResumeService
{
    private readonly IProjectRepository _projects;
    private readonly IComponentRepository _components;
    private readonly ILearningTrackRepository _tracks;
    private readonly ILearningModuleRepository _modules;
    private readonly IWorklogRepository _worklogs;
    private readonly INextStepRepository _steps;
    private readonly IDecisionRepository _decisions;
    private readonly IResourceRepository _resources;
    private readonly IIdeaRepository _ideas;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public ResumeService(
        IProjectRepository projects,
        IComponentRepository components,
        ILearningTrackRepository tracks,
        ILearningModuleRepository modules,
        IWorklogRepository worklogs,
        INextStepRepository steps,
        IDecisionRepository decisions,
        IResourceRepository resources,
        IIdeaRepository ideas,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _projects = projects;
        _components = components;
        _tracks = tracks;
        _modules = modules;
        _worklogs = worklogs;
        _steps = steps;
        _decisions = decisions;
        _resources = resources;
        _ideas = ideas;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ResumeProjectResponse> GetForProjectAsync(int projectId, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();

        var project = await _projects.GetByIdAsync(projectId, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Project not found.");

        var components = await _components.ListByProjectAsync(projectId, userId, includeDeleted: false, ct);
        var componentIds = components.Select(c => c.Id).ToArray();
        var scope = OwnerScope.ForProject(projectId, componentIds);

        var recentWorklogs = await _worklogs.ListByScopeAsync(userId, scope, take: 5, includeDeleted: false, ct);
        var openNextSteps = await _steps.ListByScopeAsync(userId, scope, openOnly: true, take: null, includeDeleted: false, ct);
        var recentDecisions = await _decisions.ListByScopeAsync(userId, scope, take: 3, includeDeleted: false, ct);
        var resources = await _resources.ListByScopeAsync(userId, scope, includeDeleted: false, ct);
        var recentIdeas = await _ideas.ListByScopeAsync(userId, scope, unconvertedOnly: true, take: 5, includeDeleted: false, ct);

        return new ResumeProjectResponse
        {
            Project = _mapper.Map<ProjectResponse>(project),
            Components = components.Select(c => _mapper.Map<ComponentResponse>(c)).ToList(),
            RecentWorklogs = recentWorklogs.Select(w => _mapper.Map<WorklogResponse>(w)).ToList(),
            OpenNextSteps = openNextSteps.Select(s => _mapper.Map<NextStepResponse>(s)).ToList(),
            RecentDecisions = recentDecisions.Select(d => _mapper.Map<DecisionResponse>(d)).ToList(),
            Resources = resources.Select(r => _mapper.Map<ResourceResponse>(r)).ToList(),
            RecentIdeas = recentIdeas.Select(i => _mapper.Map<IdeaResponse>(i)).ToList(),
            DaysSinceLastActivity = DaysSince(project.LastActivityAt)
        };
    }

    public async Task<ResumeComponentResponse> GetForComponentAsync(int componentId, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();

        var component = await _components.GetByIdAsync(componentId, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Component not found.");

        var scope = OwnerScope.ForComponent(componentId);

        var recentWorklogs = await _worklogs.ListByScopeAsync(userId, scope, take: 5, includeDeleted: false, ct);
        var openNextSteps = await _steps.ListByScopeAsync(userId, scope, openOnly: true, take: null, includeDeleted: false, ct);
        var recentDecisions = await _decisions.ListByScopeAsync(userId, scope, take: 3, includeDeleted: false, ct);
        var resources = await _resources.ListByScopeAsync(userId, scope, includeDeleted: false, ct);
        var recentIdeas = await _ideas.ListByScopeAsync(userId, scope, unconvertedOnly: true, take: 5, includeDeleted: false, ct);

        return new ResumeComponentResponse
        {
            Component = _mapper.Map<ComponentResponse>(component),
            RecentWorklogs = recentWorklogs.Select(w => _mapper.Map<WorklogResponse>(w)).ToList(),
            OpenNextSteps = openNextSteps.Select(s => _mapper.Map<NextStepResponse>(s)).ToList(),
            RecentDecisions = recentDecisions.Select(d => _mapper.Map<DecisionResponse>(d)).ToList(),
            Resources = resources.Select(r => _mapper.Map<ResourceResponse>(r)).ToList(),
            RecentIdeas = recentIdeas.Select(i => _mapper.Map<IdeaResponse>(i)).ToList(),
            DaysSinceLastActivity = DaysSince(component.LastActivityAt)
        };
    }

    public async Task<ResumeLearningTrackResponse> GetForLearningTrackAsync(int trackId, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();

        var track = await _tracks.GetByIdAsync(trackId, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Learning track not found.");

        var modules = await _modules.ListByTrackAsync(trackId, userId, includeDeleted: false, ct);
        var moduleIds = modules.Select(m => m.Id).ToArray();
        var scope = OwnerScope.ForLearningTrack(trackId, moduleIds);

        var recentWorklogs = await _worklogs.ListByScopeAsync(userId, scope, take: 5, includeDeleted: false, ct);
        var openNextSteps = await _steps.ListByScopeAsync(userId, scope, openOnly: true, take: null, includeDeleted: false, ct);
        var recentDecisions = await _decisions.ListByScopeAsync(userId, scope, take: 3, includeDeleted: false, ct);
        var resources = await _resources.ListByScopeAsync(userId, scope, includeDeleted: false, ct);
        var recentIdeas = await _ideas.ListByScopeAsync(userId, scope, unconvertedOnly: true, take: 5, includeDeleted: false, ct);

        var (total, completed) = await _modules.GetModuleProgressAsync(trackId, userId, ct);
        var progress = total == 0 ? 0m : decimal.Round((decimal)completed * 100m / total, 2);

        return new ResumeLearningTrackResponse
        {
            Track = _mapper.Map<LearningTrackResponse>(track),
            Modules = modules.Select(m => _mapper.Map<LearningModuleResponse>(m)).ToList(),
            RecentWorklogs = recentWorklogs.Select(w => _mapper.Map<WorklogResponse>(w)).ToList(),
            OpenNextSteps = openNextSteps.Select(s => _mapper.Map<NextStepResponse>(s)).ToList(),
            RecentDecisions = recentDecisions.Select(d => _mapper.Map<DecisionResponse>(d)).ToList(),
            Resources = resources.Select(r => _mapper.Map<ResourceResponse>(r)).ToList(),
            RecentIdeas = recentIdeas.Select(i => _mapper.Map<IdeaResponse>(i)).ToList(),
            ProgressPercent = progress,
            DaysSinceLastActivity = DaysSince(track.LastActivityAt)
        };
    }

    public async Task<ResumeLearningModuleResponse> GetForLearningModuleAsync(int moduleId, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();

        var module = await _modules.GetByIdAsync(moduleId, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Learning module not found.");

        var scope = OwnerScope.ForLearningModule(moduleId);

        var recentWorklogs = await _worklogs.ListByScopeAsync(userId, scope, take: 5, includeDeleted: false, ct);
        var openNextSteps = await _steps.ListByScopeAsync(userId, scope, openOnly: true, take: null, includeDeleted: false, ct);
        var recentDecisions = await _decisions.ListByScopeAsync(userId, scope, take: 3, includeDeleted: false, ct);
        var resources = await _resources.ListByScopeAsync(userId, scope, includeDeleted: false, ct);
        var recentIdeas = await _ideas.ListByScopeAsync(userId, scope, unconvertedOnly: true, take: 5, includeDeleted: false, ct);

        return new ResumeLearningModuleResponse
        {
            Module = _mapper.Map<LearningModuleResponse>(module),
            RecentWorklogs = recentWorklogs.Select(w => _mapper.Map<WorklogResponse>(w)).ToList(),
            OpenNextSteps = openNextSteps.Select(s => _mapper.Map<NextStepResponse>(s)).ToList(),
            RecentDecisions = recentDecisions.Select(d => _mapper.Map<DecisionResponse>(d)).ToList(),
            Resources = resources.Select(r => _mapper.Map<ResourceResponse>(r)).ToList(),
            RecentIdeas = recentIdeas.Select(i => _mapper.Map<IdeaResponse>(i)).ToList(),
            DaysSinceLastActivity = DaysSince(module.LastActivityAt)
        };
    }

    private static int? DaysSince(DateTime? lastActivity)
    {
        if (!lastActivity.HasValue) return null;
        return (int)Math.Floor((DateTime.UtcNow - lastActivity.Value).TotalDays);
    }
}
