using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.LearningTracks;

namespace DevTrack.Service.Interfaces;

public interface ILearningTrackService
{
    Task<PagedResult<LearningTrackResponse>> ListAsync(LearningTrackListQuery query, CancellationToken ct = default);
    Task<LearningTrackResponse> GetAsync(int id, bool includeDeleted, CancellationToken ct = default);
    Task<LearningTrackResponse> CreateAsync(LearningTrackCreateRequest request, CancellationToken ct = default);
    Task<LearningTrackResponse> UpdateAsync(int id, LearningTrackUpdateRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<LearningTrackResponse> UpdateStatusAsync(int id, LearningTrackStatusUpdateRequest request, CancellationToken ct = default);
    Task AttachTagAsync(int trackId, int tagId, CancellationToken ct = default);
    Task DetachTagAsync(int trackId, int tagId, CancellationToken ct = default);
}
