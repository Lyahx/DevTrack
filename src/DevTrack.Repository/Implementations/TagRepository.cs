using DevTrack.Domain.Entities;
using DevTrack.Infrastructure.Data;
using DevTrack.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Repository.Implementations;

public class TagRepository : ITagRepository
{
    private readonly DevTrackDbContext _db;

    public TagRepository(DevTrackDbContext db)
    {
        _db = db;
    }

    public Task<Tag?> GetByIdAsync(int id, int userId, CancellationToken ct = default)
        => _db.Tags.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId, ct);

    public async Task<IReadOnlyList<Tag>> ListAsync(int userId, CancellationToken ct = default)
        => await _db.Tags.Where(t => t.UserId == userId).OrderBy(t => t.Name).ToListAsync(ct);

    public Task<bool> NameExistsAsync(int userId, string name, int? excludeId, CancellationToken ct = default)
    {
        var q = _db.Tags.Where(t => t.UserId == userId && t.Name == name);
        if (excludeId.HasValue) q = q.Where(t => t.Id != excludeId.Value);
        return q.AnyAsync(ct);
    }

    public async Task AddAsync(Tag tag, CancellationToken ct = default) => await _db.Tags.AddAsync(tag, ct);

    public void Update(Tag tag) => _db.Tags.Update(tag);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);

    public Task<bool> ProjectTagExistsAsync(int projectId, int tagId, CancellationToken ct = default)
        => _db.ProjectTags.AnyAsync(x => x.ProjectId == projectId && x.TagId == tagId, ct);

    public async Task AddProjectTagAsync(ProjectTag link, CancellationToken ct = default)
        => await _db.ProjectTags.AddAsync(link, ct);

    public async Task RemoveProjectTagAsync(int projectId, int tagId, CancellationToken ct = default)
    {
        var link = await _db.ProjectTags.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.TagId == tagId, ct);
        if (link is not null) _db.ProjectTags.Remove(link);
    }

    public Task<bool> ComponentTagExistsAsync(int componentId, int tagId, CancellationToken ct = default)
        => _db.ComponentTags.AnyAsync(x => x.ComponentId == componentId && x.TagId == tagId, ct);

    public async Task AddComponentTagAsync(ComponentTag link, CancellationToken ct = default)
        => await _db.ComponentTags.AddAsync(link, ct);

    public async Task RemoveComponentTagAsync(int componentId, int tagId, CancellationToken ct = default)
    {
        var link = await _db.ComponentTags.FirstOrDefaultAsync(x => x.ComponentId == componentId && x.TagId == tagId, ct);
        if (link is not null) _db.ComponentTags.Remove(link);
    }

    public Task<bool> LearningTrackTagExistsAsync(int trackId, int tagId, CancellationToken ct = default)
        => _db.LearningTrackTags.AnyAsync(x => x.LearningTrackId == trackId && x.TagId == tagId, ct);

    public async Task AddLearningTrackTagAsync(LearningTrackTag link, CancellationToken ct = default)
        => await _db.LearningTrackTags.AddAsync(link, ct);

    public async Task RemoveLearningTrackTagAsync(int trackId, int tagId, CancellationToken ct = default)
    {
        var link = await _db.LearningTrackTags.FirstOrDefaultAsync(x => x.LearningTrackId == trackId && x.TagId == tagId, ct);
        if (link is not null) _db.LearningTrackTags.Remove(link);
    }
}
