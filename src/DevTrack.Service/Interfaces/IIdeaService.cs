using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.Ideas;
using DevTrack.Domain.DTOs.NextSteps;

namespace DevTrack.Service.Interfaces;

public interface IIdeaService
{
    Task<PagedResult<IdeaResponse>> ListAsync(IdeaListQuery query, CancellationToken ct = default);
    Task<IdeaResponse> GetAsync(int id, bool includeDeleted, CancellationToken ct = default);
    Task<IdeaResponse> CreateAsync(IdeaCreateRequest request, CancellationToken ct = default);
    Task<IdeaResponse> UpdateAsync(int id, IdeaUpdateRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<NextStepResponse> ConvertToNextStepAsync(int id, IdeaConvertRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<IdeaResponse>> ListForProjectAsync(int projectId, bool includeDeleted, CancellationToken ct = default);
    Task<IReadOnlyList<IdeaResponse>> ListForComponentAsync(int componentId, bool includeDeleted, CancellationToken ct = default);
    Task<IReadOnlyList<IdeaResponse>> ListForLearningTrackAsync(int trackId, bool includeDeleted, CancellationToken ct = default);
    Task<IReadOnlyList<IdeaResponse>> ListForLearningModuleAsync(int moduleId, bool includeDeleted, CancellationToken ct = default);
}
