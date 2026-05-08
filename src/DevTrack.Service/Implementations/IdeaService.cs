using AutoMapper;
using DevTrack.Domain.Common;
using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.Ideas;
using DevTrack.Domain.DTOs.NextSteps;
using DevTrack.Domain.Entities;
using DevTrack.Domain.Exceptions;
using DevTrack.Repository.Common;
using DevTrack.Repository.Interfaces;
using DevTrack.Service.Common;
using DevTrack.Service.Interfaces;

namespace DevTrack.Service.Implementations;

public class IdeaService : IIdeaService
{
    private readonly IIdeaRepository _ideas;
    private readonly INextStepRepository _steps;
    private readonly IComponentRepository _components;
    private readonly ILearningModuleRepository _modules;
    private readonly IOwnerValidator _owners;
    private readonly IActivityTrackingService _activity;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public IdeaService(
        IIdeaRepository ideas,
        INextStepRepository steps,
        IComponentRepository components,
        ILearningModuleRepository modules,
        IOwnerValidator owners,
        IActivityTrackingService activity,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _ideas = ideas;
        _steps = steps;
        _components = components;
        _modules = modules;
        _owners = owners;
        _activity = activity;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<PagedResult<IdeaResponse>> ListAsync(IdeaListQuery query, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize is < 1 or > 100 ? 20 : query.PageSize;
        query.Page = page; query.PageSize = pageSize;

        var (items, total) = await _ideas.ListAsync(userId, query, ct);
        return new PagedResult<IdeaResponse>
        {
            Items = items.Select(i => _mapper.Map<IdeaResponse>(i)).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<IdeaResponse> GetAsync(int id, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var idea = await _ideas.GetByIdAsync(id, userId, includeDeleted, ct)
            ?? throw new NotFoundException("Idea not found.");
        return _mapper.Map<IdeaResponse>(idea);
    }

    public async Task<IdeaResponse> CreateAsync(IdeaCreateRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        await _owners.EnsureOwnedByUserAsync(request.Owner, userId, ct);

        var idea = new Idea
        {
            UserId = userId,
            Content = request.Content,
            CapturedAt = DateTime.UtcNow
        };
        idea.ApplyOwner(request.Owner);

        await _ideas.AddAsync(idea, ct);
        await _ideas.SaveChangesAsync(ct);

        await _activity.RecordActivityAsync(request.Owner, userId, ct);
        return _mapper.Map<IdeaResponse>(idea);
    }

    public async Task<IdeaResponse> UpdateAsync(int id, IdeaUpdateRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var idea = await _ideas.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Idea not found.");

        idea.Content = request.Content;
        await _ideas.SaveChangesAsync(ct);
        return _mapper.Map<IdeaResponse>(idea);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var idea = await _ideas.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Idea not found.");

        idea.IsDeleted = true;
        idea.DeletedAt = DateTime.UtcNow;
        await _ideas.SaveChangesAsync(ct);
    }

    public async Task<NextStepResponse> ConvertToNextStepAsync(int id, IdeaConvertRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var idea = await _ideas.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Idea not found.");
        if (idea.IsConvertedToNextStep)
            throw new ConflictException("Idea has already been converted.");

        var owner = OwnerReference.FromColumns(idea.ProjectId, idea.ComponentId, idea.LearningTrackId, idea.LearningModuleId);

        var step = new NextStep
        {
            UserId = userId,
            Description = idea.Content,
            Priority = request.Priority
        };
        step.ApplyOwner(owner);

        await _steps.AddAsync(step, ct);
        await _steps.SaveChangesAsync(ct);

        idea.IsConvertedToNextStep = true;
        idea.ConvertedNextStepId = step.Id;
        await _ideas.SaveChangesAsync(ct);

        await _activity.RecordActivityAsync(owner, userId, ct);
        return _mapper.Map<NextStepResponse>(step);
    }

    public async Task<IReadOnlyList<IdeaResponse>> ListForProjectAsync(int projectId, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var componentIds = await _components.GetComponentIdsForProjectAsync(projectId, userId, includeDeleted, ct);
        var items = await _ideas.ListByScopeAsync(userId, OwnerScope.ForProject(projectId, componentIds), null, null, includeDeleted, ct);
        return items.Select(i => _mapper.Map<IdeaResponse>(i)).ToList();
    }

    public async Task<IReadOnlyList<IdeaResponse>> ListForComponentAsync(int componentId, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var items = await _ideas.ListByScopeAsync(userId, OwnerScope.ForComponent(componentId), null, null, includeDeleted, ct);
        return items.Select(i => _mapper.Map<IdeaResponse>(i)).ToList();
    }

    public async Task<IReadOnlyList<IdeaResponse>> ListForLearningTrackAsync(int trackId, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var moduleIds = await _modules.GetModuleIdsForTrackAsync(trackId, userId, includeDeleted, ct);
        var items = await _ideas.ListByScopeAsync(userId, OwnerScope.ForLearningTrack(trackId, moduleIds), null, null, includeDeleted, ct);
        return items.Select(i => _mapper.Map<IdeaResponse>(i)).ToList();
    }

    public async Task<IReadOnlyList<IdeaResponse>> ListForLearningModuleAsync(int moduleId, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var items = await _ideas.ListByScopeAsync(userId, OwnerScope.ForLearningModule(moduleId), null, null, includeDeleted, ct);
        return items.Select(i => _mapper.Map<IdeaResponse>(i)).ToList();
    }
}
