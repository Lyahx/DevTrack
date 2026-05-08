using DevTrack.Domain.DTOs.Commits;

namespace DevTrack.Service.Interfaces;

public interface ICommitService
{
    Task<CommitListResponse> GetForProjectAsync(int projectId, int take, CancellationToken ct = default);
}
