using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.Projects;

namespace DevTrack.Service.Interfaces;

public interface IProjectService
{
    Task<PagedResult<ProjectResponse>> ListAsync(ProjectListQuery query, CancellationToken ct = default);
    Task<ProjectResponse> GetAsync(int id, bool includeDeleted, CancellationToken ct = default);
    Task<ProjectResponse> CreateAsync(ProjectCreateRequest request, CancellationToken ct = default);
    Task<ProjectResponse> UpdateAsync(int id, ProjectUpdateRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<ProjectResponse> UpdateStatusAsync(int id, ProjectStatusUpdateRequest request, CancellationToken ct = default);
    Task AttachTagAsync(int projectId, int tagId, CancellationToken ct = default);
    Task DetachTagAsync(int projectId, int tagId, CancellationToken ct = default);
}
