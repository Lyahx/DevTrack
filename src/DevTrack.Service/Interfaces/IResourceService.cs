using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.Resources;

namespace DevTrack.Service.Interfaces;

public interface IResourceService
{
    Task<PagedResult<ResourceResponse>> ListAsync(ResourceListQuery query, CancellationToken ct = default);
    Task<ResourceResponse> GetAsync(int id, bool includeDeleted, CancellationToken ct = default);
    Task<ResourceResponse> CreateAsync(ResourceCreateRequest request, CancellationToken ct = default);
    Task<ResourceResponse> UpdateAsync(int id, ResourceUpdateRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<ResourceResponse>> ListForProjectAsync(int projectId, bool includeDeleted, CancellationToken ct = default);
    Task<IReadOnlyList<ResourceResponse>> ListForComponentAsync(int componentId, bool includeDeleted, CancellationToken ct = default);
    Task<IReadOnlyList<ResourceResponse>> ListForLearningTrackAsync(int trackId, bool includeDeleted, CancellationToken ct = default);
    Task<IReadOnlyList<ResourceResponse>> ListForLearningModuleAsync(int moduleId, bool includeDeleted, CancellationToken ct = default);
}
