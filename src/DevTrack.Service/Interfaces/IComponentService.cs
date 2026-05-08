using DevTrack.Domain.DTOs.Components;

namespace DevTrack.Service.Interfaces;

public interface IComponentService
{
    Task<IReadOnlyList<ComponentResponse>> ListByProjectAsync(int projectId, bool includeDeleted, CancellationToken ct = default);
    Task<ComponentResponse> GetAsync(int id, bool includeDeleted, CancellationToken ct = default);
    Task<ComponentResponse> CreateAsync(int projectId, ComponentCreateRequest request, CancellationToken ct = default);
    Task<ComponentResponse> UpdateAsync(int id, ComponentUpdateRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<ComponentResponse> UpdateStatusNoteAsync(int id, ComponentStatusNoteRequest request, CancellationToken ct = default);
    Task AttachTagAsync(int componentId, int tagId, CancellationToken ct = default);
    Task DetachTagAsync(int componentId, int tagId, CancellationToken ct = default);
}
