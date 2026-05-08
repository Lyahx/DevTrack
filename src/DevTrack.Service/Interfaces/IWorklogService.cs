using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.Worklogs;

namespace DevTrack.Service.Interfaces;

public interface IWorklogService
{
    Task<PagedResult<WorklogResponse>> ListAsync(WorklogListQuery query, CancellationToken ct = default);
    Task<WorklogResponse> GetAsync(int id, bool includeDeleted, CancellationToken ct = default);
    Task<WorklogResponse> CreateAsync(WorklogCreateRequest request, CancellationToken ct = default);
    Task<WorklogResponse> UpdateAsync(int id, WorklogUpdateRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<WorklogResponse>> ListRecentAsync(int days, CancellationToken ct = default);
    Task<IReadOnlyList<WorklogResponse>> ListForProjectAsync(int projectId, bool includeDeleted, CancellationToken ct = default);
    Task<IReadOnlyList<WorklogResponse>> ListForComponentAsync(int componentId, bool includeDeleted, CancellationToken ct = default);
    Task<IReadOnlyList<WorklogResponse>> ListForLearningTrackAsync(int trackId, bool includeDeleted, CancellationToken ct = default);
    Task<IReadOnlyList<WorklogResponse>> ListForLearningModuleAsync(int moduleId, bool includeDeleted, CancellationToken ct = default);
}
