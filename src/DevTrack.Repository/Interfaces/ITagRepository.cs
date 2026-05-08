using DevTrack.Domain.Entities;

namespace DevTrack.Repository.Interfaces;

public interface ITagRepository
{
    Task<Tag?> GetByIdAsync(int id, int userId, CancellationToken ct = default);
    Task<IReadOnlyList<Tag>> ListAsync(int userId, CancellationToken ct = default);
    Task<bool> NameExistsAsync(int userId, string name, int? excludeId, CancellationToken ct = default);
    Task AddAsync(Tag tag, CancellationToken ct = default);
    void Update(Tag tag);
    Task SaveChangesAsync(CancellationToken ct = default);

    // Junctions
    Task<bool> ProjectTagExistsAsync(int projectId, int tagId, CancellationToken ct = default);
    Task AddProjectTagAsync(ProjectTag link, CancellationToken ct = default);
    Task RemoveProjectTagAsync(int projectId, int tagId, CancellationToken ct = default);

    Task<bool> ComponentTagExistsAsync(int componentId, int tagId, CancellationToken ct = default);
    Task AddComponentTagAsync(ComponentTag link, CancellationToken ct = default);
    Task RemoveComponentTagAsync(int componentId, int tagId, CancellationToken ct = default);

    Task<bool> LearningTrackTagExistsAsync(int trackId, int tagId, CancellationToken ct = default);
    Task AddLearningTrackTagAsync(LearningTrackTag link, CancellationToken ct = default);
    Task RemoveLearningTrackTagAsync(int trackId, int tagId, CancellationToken ct = default);
}
