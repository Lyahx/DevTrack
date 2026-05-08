using DevTrack.Domain.DTOs.LearningTracks;

namespace DevTrack.Service.Interfaces;

public interface ILearningModuleService
{
    Task<IReadOnlyList<LearningModuleResponse>> ListByTrackAsync(int trackId, bool includeDeleted, CancellationToken ct = default);
    Task<LearningModuleResponse> GetAsync(int id, bool includeDeleted, CancellationToken ct = default);
    Task<LearningModuleResponse> CreateAsync(int trackId, LearningModuleCreateRequest request, CancellationToken ct = default);
    Task<LearningModuleResponse> UpdateAsync(int id, LearningModuleUpdateRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<LearningModuleResponse> UpdateStatusAsync(int id, LearningModuleStatusUpdateRequest request, CancellationToken ct = default);
    Task<LearningModuleResponse> UpdateOrderAsync(int id, LearningModuleOrderUpdateRequest request, CancellationToken ct = default);
}
