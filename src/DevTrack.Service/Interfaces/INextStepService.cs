using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.NextSteps;

namespace DevTrack.Service.Interfaces;

public interface INextStepService
{
    Task<PagedResult<NextStepResponse>> ListAsync(NextStepListQuery query, CancellationToken ct = default);
    Task<NextStepResponse> GetAsync(int id, bool includeDeleted, CancellationToken ct = default);
    Task<NextStepResponse> CreateAsync(NextStepCreateRequest request, CancellationToken ct = default);
    Task<NextStepResponse> UpdateAsync(int id, NextStepUpdateRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<NextStepResponse> CompleteAsync(int id, CancellationToken ct = default);
    Task<NextStepResponse> UncompleteAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<NextStepResponse>> ListOpenAsync(CancellationToken ct = default);
    Task<IReadOnlyList<NextStepResponse>> ListForProjectAsync(int projectId, bool includeDeleted, CancellationToken ct = default);
    Task<IReadOnlyList<NextStepResponse>> ListForComponentAsync(int componentId, bool includeDeleted, CancellationToken ct = default);
    Task<IReadOnlyList<NextStepResponse>> ListForLearningTrackAsync(int trackId, bool includeDeleted, CancellationToken ct = default);
    Task<IReadOnlyList<NextStepResponse>> ListForLearningModuleAsync(int moduleId, bool includeDeleted, CancellationToken ct = default);
}
