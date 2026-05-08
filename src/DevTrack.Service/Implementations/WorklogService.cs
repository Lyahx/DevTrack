using AutoMapper;
using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.Worklogs;
using DevTrack.Domain.Entities;
using DevTrack.Domain.Exceptions;
using DevTrack.Repository.Common;
using DevTrack.Repository.Interfaces;
using DevTrack.Service.Common;
using DevTrack.Service.Interfaces;

namespace DevTrack.Service.Implementations;

public class WorklogService : IWorklogService
{
    private readonly IWorklogRepository _worklogs;
    private readonly IComponentRepository _components;
    private readonly ILearningModuleRepository _modules;
    private readonly IOwnerValidator _owners;
    private readonly IActivityTrackingService _activity;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public WorklogService(
        IWorklogRepository worklogs,
        IComponentRepository components,
        ILearningModuleRepository modules,
        IOwnerValidator owners,
        IActivityTrackingService activity,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _worklogs = worklogs;
        _components = components;
        _modules = modules;
        _owners = owners;
        _activity = activity;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<PagedResult<WorklogResponse>> ListAsync(WorklogListQuery query, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize is < 1 or > 100 ? 20 : query.PageSize;
        query.Page = page;
        query.PageSize = pageSize;

        var (items, total) = await _worklogs.ListAsync(userId, query, ct);
        return new PagedResult<WorklogResponse>
        {
            Items = items.Select(w => _mapper.Map<WorklogResponse>(w)).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<WorklogResponse> GetAsync(int id, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var worklog = await _worklogs.GetByIdAsync(id, userId, includeDeleted, ct)
            ?? throw new NotFoundException("Worklog not found.");
        return _mapper.Map<WorklogResponse>(worklog);
    }

    public async Task<WorklogResponse> CreateAsync(WorklogCreateRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        await _owners.EnsureOwnedByUserAsync(request.Owner, userId, ct);

        var worklog = new Worklog
        {
            UserId = userId,
            WhatIDid = request.WhatIDid,
            WhatsLeft = request.WhatsLeft,
            Reasoning = request.Reasoning,
            Alternatives = request.Alternatives,
            LoggedAt = request.LoggedAt ?? DateTime.UtcNow
        };
        worklog.ApplyOwner(request.Owner);

        await _worklogs.AddAsync(worklog, ct);
        await _worklogs.SaveChangesAsync(ct);

        await _activity.RecordActivityAsync(request.Owner, userId, ct);

        return _mapper.Map<WorklogResponse>(worklog);
    }

    public async Task<WorklogResponse> UpdateAsync(int id, WorklogUpdateRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var worklog = await _worklogs.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Worklog not found.");

        worklog.WhatIDid = request.WhatIDid;
        worklog.WhatsLeft = request.WhatsLeft;
        worklog.Reasoning = request.Reasoning;
        worklog.Alternatives = request.Alternatives;
        if (request.LoggedAt.HasValue) worklog.LoggedAt = request.LoggedAt.Value;

        await _worklogs.SaveChangesAsync(ct);
        return _mapper.Map<WorklogResponse>(worklog);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var worklog = await _worklogs.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Worklog not found.");

        worklog.IsDeleted = true;
        worklog.DeletedAt = DateTime.UtcNow;
        await _worklogs.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<WorklogResponse>> ListRecentAsync(int days, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var items = await _worklogs.ListRecentAsync(userId, days, ct);
        return items.Select(w => _mapper.Map<WorklogResponse>(w)).ToList();
    }

    public async Task<IReadOnlyList<WorklogResponse>> ListForProjectAsync(int projectId, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var componentIds = await _components.GetComponentIdsForProjectAsync(projectId, userId, includeDeleted, ct);
        var scope = OwnerScope.ForProject(projectId, componentIds);
        var items = await _worklogs.ListByScopeAsync(userId, scope, take: null, includeDeleted, ct);
        return items.Select(w => _mapper.Map<WorklogResponse>(w)).ToList();
    }

    public async Task<IReadOnlyList<WorklogResponse>> ListForComponentAsync(int componentId, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var scope = OwnerScope.ForComponent(componentId);
        var items = await _worklogs.ListByScopeAsync(userId, scope, take: null, includeDeleted, ct);
        return items.Select(w => _mapper.Map<WorklogResponse>(w)).ToList();
    }

    public async Task<IReadOnlyList<WorklogResponse>> ListForLearningTrackAsync(int trackId, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var moduleIds = await _modules.GetModuleIdsForTrackAsync(trackId, userId, includeDeleted, ct);
        var scope = OwnerScope.ForLearningTrack(trackId, moduleIds);
        var items = await _worklogs.ListByScopeAsync(userId, scope, take: null, includeDeleted, ct);
        return items.Select(w => _mapper.Map<WorklogResponse>(w)).ToList();
    }

    public async Task<IReadOnlyList<WorklogResponse>> ListForLearningModuleAsync(int moduleId, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var scope = OwnerScope.ForLearningModule(moduleId);
        var items = await _worklogs.ListByScopeAsync(userId, scope, take: null, includeDeleted, ct);
        return items.Select(w => _mapper.Map<WorklogResponse>(w)).ToList();
    }
}
