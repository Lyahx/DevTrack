using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.Decisions;

namespace DevTrack.Service.Interfaces;

public interface IDecisionService
{
    Task<PagedResult<DecisionResponse>> ListAsync(DecisionListQuery query, CancellationToken ct = default);
    Task<DecisionResponse> GetAsync(int id, bool includeDeleted, CancellationToken ct = default);
    Task<DecisionResponse> CreateAsync(DecisionCreateRequest request, CancellationToken ct = default);
    Task<DecisionResponse> UpdateAsync(int id, DecisionUpdateRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<DecisionResponse>> ListForProjectAsync(int projectId, bool includeDeleted, CancellationToken ct = default);
    Task<IReadOnlyList<DecisionResponse>> ListForComponentAsync(int componentId, bool includeDeleted, CancellationToken ct = default);
    Task<IReadOnlyList<DecisionResponse>> ListForLearningTrackAsync(int trackId, bool includeDeleted, CancellationToken ct = default);
    Task<IReadOnlyList<DecisionResponse>> ListForLearningModuleAsync(int moduleId, bool includeDeleted, CancellationToken ct = default);
}
