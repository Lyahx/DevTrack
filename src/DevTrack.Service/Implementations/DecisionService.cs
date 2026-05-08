using AutoMapper;
using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.Decisions;
using DevTrack.Domain.Entities;
using DevTrack.Domain.Exceptions;
using DevTrack.Repository.Common;
using DevTrack.Repository.Interfaces;
using DevTrack.Service.Common;
using DevTrack.Service.Interfaces;

namespace DevTrack.Service.Implementations;

public class DecisionService : IDecisionService
{
    private readonly IDecisionRepository _decisions;
    private readonly IComponentRepository _components;
    private readonly ILearningModuleRepository _modules;
    private readonly IOwnerValidator _owners;
    private readonly IActivityTrackingService _activity;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public DecisionService(
        IDecisionRepository decisions,
        IComponentRepository components,
        ILearningModuleRepository modules,
        IOwnerValidator owners,
        IActivityTrackingService activity,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _decisions = decisions;
        _components = components;
        _modules = modules;
        _owners = owners;
        _activity = activity;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<PagedResult<DecisionResponse>> ListAsync(DecisionListQuery query, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize is < 1 or > 100 ? 20 : query.PageSize;
        query.Page = page; query.PageSize = pageSize;

        var (items, total) = await _decisions.ListAsync(userId, query, ct);
        return new PagedResult<DecisionResponse>
        {
            Items = items.Select(d => _mapper.Map<DecisionResponse>(d)).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<DecisionResponse> GetAsync(int id, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var decision = await _decisions.GetByIdAsync(id, userId, includeDeleted, ct)
            ?? throw new NotFoundException("Decision not found.");
        return _mapper.Map<DecisionResponse>(decision);
    }

    public async Task<DecisionResponse> CreateAsync(DecisionCreateRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        await _owners.EnsureOwnedByUserAsync(request.Owner, userId, ct);

        var decision = new Decision
        {
            UserId = userId,
            Title = request.Title,
            Reasoning = request.Reasoning,
            Alternatives = request.Alternatives,
            DecidedAt = request.DecidedAt ?? DateTime.UtcNow
        };
        decision.ApplyOwner(request.Owner);

        await _decisions.AddAsync(decision, ct);
        await _decisions.SaveChangesAsync(ct);

        await _activity.RecordActivityAsync(request.Owner, userId, ct);
        return _mapper.Map<DecisionResponse>(decision);
    }

    public async Task<DecisionResponse> UpdateAsync(int id, DecisionUpdateRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var decision = await _decisions.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Decision not found.");

        decision.Title = request.Title;
        decision.Reasoning = request.Reasoning;
        decision.Alternatives = request.Alternatives;
        if (request.DecidedAt.HasValue) decision.DecidedAt = request.DecidedAt.Value;

        await _decisions.SaveChangesAsync(ct);
        return _mapper.Map<DecisionResponse>(decision);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var decision = await _decisions.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Decision not found.");

        decision.IsDeleted = true;
        decision.DeletedAt = DateTime.UtcNow;
        await _decisions.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<DecisionResponse>> ListForProjectAsync(int projectId, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var componentIds = await _components.GetComponentIdsForProjectAsync(projectId, userId, includeDeleted, ct);
        var items = await _decisions.ListByScopeAsync(userId, OwnerScope.ForProject(projectId, componentIds), null, includeDeleted, ct);
        return items.Select(d => _mapper.Map<DecisionResponse>(d)).ToList();
    }

    public async Task<IReadOnlyList<DecisionResponse>> ListForComponentAsync(int componentId, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var items = await _decisions.ListByScopeAsync(userId, OwnerScope.ForComponent(componentId), null, includeDeleted, ct);
        return items.Select(d => _mapper.Map<DecisionResponse>(d)).ToList();
    }

    public async Task<IReadOnlyList<DecisionResponse>> ListForLearningTrackAsync(int trackId, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var moduleIds = await _modules.GetModuleIdsForTrackAsync(trackId, userId, includeDeleted, ct);
        var items = await _decisions.ListByScopeAsync(userId, OwnerScope.ForLearningTrack(trackId, moduleIds), null, includeDeleted, ct);
        return items.Select(d => _mapper.Map<DecisionResponse>(d)).ToList();
    }

    public async Task<IReadOnlyList<DecisionResponse>> ListForLearningModuleAsync(int moduleId, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var items = await _decisions.ListByScopeAsync(userId, OwnerScope.ForLearningModule(moduleId), null, includeDeleted, ct);
        return items.Select(d => _mapper.Map<DecisionResponse>(d)).ToList();
    }
}
