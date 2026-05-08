using AutoMapper;
using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.Resources;
using DevTrack.Domain.Entities;
using DevTrack.Domain.Exceptions;
using DevTrack.Repository.Common;
using DevTrack.Repository.Interfaces;
using DevTrack.Service.Common;
using DevTrack.Service.Interfaces;

namespace DevTrack.Service.Implementations;

public class ResourceService : IResourceService
{
    private readonly IResourceRepository _resources;
    private readonly IComponentRepository _components;
    private readonly ILearningModuleRepository _modules;
    private readonly IOwnerValidator _owners;
    private readonly IActivityTrackingService _activity;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public ResourceService(
        IResourceRepository resources,
        IComponentRepository components,
        ILearningModuleRepository modules,
        IOwnerValidator owners,
        IActivityTrackingService activity,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _resources = resources;
        _components = components;
        _modules = modules;
        _owners = owners;
        _activity = activity;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<PagedResult<ResourceResponse>> ListAsync(ResourceListQuery query, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize is < 1 or > 100 ? 20 : query.PageSize;
        query.Page = page; query.PageSize = pageSize;

        var (items, total) = await _resources.ListAsync(userId, query, ct);
        return new PagedResult<ResourceResponse>
        {
            Items = items.Select(r => _mapper.Map<ResourceResponse>(r)).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<ResourceResponse> GetAsync(int id, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var res = await _resources.GetByIdAsync(id, userId, includeDeleted, ct)
            ?? throw new NotFoundException("Resource not found.");
        return _mapper.Map<ResourceResponse>(res);
    }

    public async Task<ResourceResponse> CreateAsync(ResourceCreateRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        await _owners.EnsureOwnedByUserAsync(request.Owner, userId, ct);

        var res = new Resource
        {
            UserId = userId,
            Title = request.Title,
            Url = request.Url,
            Type = request.Type,
            Notes = request.Notes,
            AddedAt = DateTime.UtcNow
        };
        res.ApplyOwner(request.Owner);

        await _resources.AddAsync(res, ct);
        await _resources.SaveChangesAsync(ct);

        await _activity.RecordActivityAsync(request.Owner, userId, ct);
        return _mapper.Map<ResourceResponse>(res);
    }

    public async Task<ResourceResponse> UpdateAsync(int id, ResourceUpdateRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var res = await _resources.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Resource not found.");

        res.Title = request.Title;
        res.Url = request.Url;
        res.Type = request.Type;
        res.Notes = request.Notes;
        await _resources.SaveChangesAsync(ct);
        return _mapper.Map<ResourceResponse>(res);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var res = await _resources.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Resource not found.");

        res.IsDeleted = true;
        res.DeletedAt = DateTime.UtcNow;
        await _resources.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<ResourceResponse>> ListForProjectAsync(int projectId, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var componentIds = await _components.GetComponentIdsForProjectAsync(projectId, userId, includeDeleted, ct);
        var items = await _resources.ListByScopeAsync(userId, OwnerScope.ForProject(projectId, componentIds), includeDeleted, ct);
        return items.Select(r => _mapper.Map<ResourceResponse>(r)).ToList();
    }

    public async Task<IReadOnlyList<ResourceResponse>> ListForComponentAsync(int componentId, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var items = await _resources.ListByScopeAsync(userId, OwnerScope.ForComponent(componentId), includeDeleted, ct);
        return items.Select(r => _mapper.Map<ResourceResponse>(r)).ToList();
    }

    public async Task<IReadOnlyList<ResourceResponse>> ListForLearningTrackAsync(int trackId, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var moduleIds = await _modules.GetModuleIdsForTrackAsync(trackId, userId, includeDeleted, ct);
        var items = await _resources.ListByScopeAsync(userId, OwnerScope.ForLearningTrack(trackId, moduleIds), includeDeleted, ct);
        return items.Select(r => _mapper.Map<ResourceResponse>(r)).ToList();
    }

    public async Task<IReadOnlyList<ResourceResponse>> ListForLearningModuleAsync(int moduleId, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var items = await _resources.ListByScopeAsync(userId, OwnerScope.ForLearningModule(moduleId), includeDeleted, ct);
        return items.Select(r => _mapper.Map<ResourceResponse>(r)).ToList();
    }
}
