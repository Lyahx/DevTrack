using AutoMapper;
using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.NextSteps;
using DevTrack.Domain.Entities;
using DevTrack.Domain.Exceptions;
using DevTrack.Repository.Common;
using DevTrack.Repository.Interfaces;
using DevTrack.Service.Common;
using DevTrack.Service.Interfaces;

namespace DevTrack.Service.Implementations;

public class NextStepService : INextStepService
{
    private readonly INextStepRepository _steps;
    private readonly IComponentRepository _components;
    private readonly ILearningModuleRepository _modules;
    private readonly IOwnerValidator _owners;
    private readonly IActivityTrackingService _activity;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public NextStepService(
        INextStepRepository steps,
        IComponentRepository components,
        ILearningModuleRepository modules,
        IOwnerValidator owners,
        IActivityTrackingService activity,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _steps = steps;
        _components = components;
        _modules = modules;
        _owners = owners;
        _activity = activity;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<PagedResult<NextStepResponse>> ListAsync(NextStepListQuery query, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize is < 1 or > 100 ? 20 : query.PageSize;
        query.Page = page; query.PageSize = pageSize;

        var (items, total) = await _steps.ListAsync(userId, query, ct);
        return new PagedResult<NextStepResponse>
        {
            Items = items.Select(s => _mapper.Map<NextStepResponse>(s)).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<NextStepResponse> GetAsync(int id, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var step = await _steps.GetByIdAsync(id, userId, includeDeleted, ct)
            ?? throw new NotFoundException("Next step not found.");
        return _mapper.Map<NextStepResponse>(step);
    }

    public async Task<NextStepResponse> CreateAsync(NextStepCreateRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        await _owners.EnsureOwnedByUserAsync(request.Owner, userId, ct);

        var step = new NextStep
        {
            UserId = userId,
            Description = request.Description,
            Priority = request.Priority
        };
        step.ApplyOwner(request.Owner);

        await _steps.AddAsync(step, ct);
        await _steps.SaveChangesAsync(ct);

        await _activity.RecordActivityAsync(request.Owner, userId, ct);
        return _mapper.Map<NextStepResponse>(step);
    }

    public async Task<NextStepResponse> UpdateAsync(int id, NextStepUpdateRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var step = await _steps.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Next step not found.");

        step.Description = request.Description;
        step.Priority = request.Priority;
        await _steps.SaveChangesAsync(ct);
        return _mapper.Map<NextStepResponse>(step);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var step = await _steps.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Next step not found.");

        step.IsDeleted = true;
        step.DeletedAt = DateTime.UtcNow;
        await _steps.SaveChangesAsync(ct);
    }

    public async Task<NextStepResponse> CompleteAsync(int id, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var step = await _steps.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Next step not found.");

        step.IsCompleted = true;
        step.CompletedAt = DateTime.UtcNow;
        await _steps.SaveChangesAsync(ct);
        return _mapper.Map<NextStepResponse>(step);
    }

    public async Task<NextStepResponse> UncompleteAsync(int id, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var step = await _steps.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Next step not found.");

        step.IsCompleted = false;
        step.CompletedAt = null;
        await _steps.SaveChangesAsync(ct);
        return _mapper.Map<NextStepResponse>(step);
    }

    public async Task<IReadOnlyList<NextStepResponse>> ListOpenAsync(CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var items = await _steps.ListOpenAsync(userId, ct);
        return items.Select(s => _mapper.Map<NextStepResponse>(s)).ToList();
    }

    public async Task<IReadOnlyList<NextStepResponse>> ListForProjectAsync(int projectId, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var componentIds = await _components.GetComponentIdsForProjectAsync(projectId, userId, includeDeleted, ct);
        var items = await _steps.ListByScopeAsync(userId, OwnerScope.ForProject(projectId, componentIds), null, null, includeDeleted, ct);
        return items.Select(s => _mapper.Map<NextStepResponse>(s)).ToList();
    }

    public async Task<IReadOnlyList<NextStepResponse>> ListForComponentAsync(int componentId, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var items = await _steps.ListByScopeAsync(userId, OwnerScope.ForComponent(componentId), null, null, includeDeleted, ct);
        return items.Select(s => _mapper.Map<NextStepResponse>(s)).ToList();
    }

    public async Task<IReadOnlyList<NextStepResponse>> ListForLearningTrackAsync(int trackId, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var moduleIds = await _modules.GetModuleIdsForTrackAsync(trackId, userId, includeDeleted, ct);
        var items = await _steps.ListByScopeAsync(userId, OwnerScope.ForLearningTrack(trackId, moduleIds), null, null, includeDeleted, ct);
        return items.Select(s => _mapper.Map<NextStepResponse>(s)).ToList();
    }

    public async Task<IReadOnlyList<NextStepResponse>> ListForLearningModuleAsync(int moduleId, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var items = await _steps.ListByScopeAsync(userId, OwnerScope.ForLearningModule(moduleId), null, null, includeDeleted, ct);
        return items.Select(s => _mapper.Map<NextStepResponse>(s)).ToList();
    }
}
