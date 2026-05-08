using AutoMapper;
using DevTrack.Domain.DTOs.Tags;
using DevTrack.Domain.Entities;
using DevTrack.Domain.Exceptions;
using DevTrack.Repository.Interfaces;
using DevTrack.Service.Interfaces;

namespace DevTrack.Service.Implementations;

public class TagService : ITagService
{
    private readonly ITagRepository _tags;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public TagService(ITagRepository tags, ICurrentUser currentUser, IMapper mapper)
    {
        _tags = tags;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<TagResponse>> ListAsync(CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var items = await _tags.ListAsync(userId, ct);
        return items.Select(t => _mapper.Map<TagResponse>(t)).ToList();
    }

    public async Task<TagResponse> CreateAsync(TagCreateRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        if (await _tags.NameExistsAsync(userId, request.Name, excludeId: null, ct))
            throw new ConflictException("A tag with that name already exists.");

        var tag = _mapper.Map<Tag>(request);
        tag.UserId = userId;
        await _tags.AddAsync(tag, ct);
        await _tags.SaveChangesAsync(ct);
        return _mapper.Map<TagResponse>(tag);
    }

    public async Task<TagResponse> UpdateAsync(int id, TagUpdateRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var tag = await _tags.GetByIdAsync(id, userId, ct)
            ?? throw new NotFoundException("Tag not found.");

        if (!string.Equals(tag.Name, request.Name, StringComparison.OrdinalIgnoreCase)
            && await _tags.NameExistsAsync(userId, request.Name, excludeId: tag.Id, ct))
            throw new ConflictException("A tag with that name already exists.");

        tag.Name = request.Name;
        tag.Color = request.Color;
        await _tags.SaveChangesAsync(ct);
        return _mapper.Map<TagResponse>(tag);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var tag = await _tags.GetByIdAsync(id, userId, ct)
            ?? throw new NotFoundException("Tag not found.");

        tag.IsDeleted = true;
        tag.DeletedAt = DateTime.UtcNow;
        await _tags.SaveChangesAsync(ct);
    }
}
