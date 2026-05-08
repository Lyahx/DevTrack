using DevTrack.Domain.DTOs.Tags;

namespace DevTrack.Service.Interfaces;

public interface ITagService
{
    Task<IReadOnlyList<TagResponse>> ListAsync(CancellationToken ct = default);
    Task<TagResponse> CreateAsync(TagCreateRequest request, CancellationToken ct = default);
    Task<TagResponse> UpdateAsync(int id, TagUpdateRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
