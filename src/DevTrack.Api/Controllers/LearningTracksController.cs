using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.LearningTracks;
using DevTrack.Service.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevTrack.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/learning-tracks")]
public class LearningTracksController : ControllerBase
{
    private readonly ILearningTrackService _service;
    private readonly IValidator<LearningTrackCreateRequest> _createValidator;
    private readonly IValidator<LearningTrackUpdateRequest> _updateValidator;
    private readonly IValidator<LearningTrackStatusUpdateRequest> _statusValidator;

    public LearningTracksController(
        ILearningTrackService service,
        IValidator<LearningTrackCreateRequest> createValidator,
        IValidator<LearningTrackUpdateRequest> updateValidator,
        IValidator<LearningTrackStatusUpdateRequest> statusValidator)
    {
        _service = service;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _statusValidator = statusValidator;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<LearningTrackResponse>>>> List(
        [FromQuery] LearningTrackListQuery query,
        CancellationToken ct)
    {
        var result = await _service.ListAsync(query, ct);
        return Ok(ApiResponse<PagedResult<LearningTrackResponse>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<LearningTrackResponse>>> Get(int id, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
    {
        var result = await _service.GetAsync(id, includeDeleted, ct);
        return Ok(ApiResponse<LearningTrackResponse>.Ok(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<LearningTrackResponse>>> Create([FromBody] LearningTrackCreateRequest request, CancellationToken ct)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);
        var result = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, ApiResponse<LearningTrackResponse>.Ok(result));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<LearningTrackResponse>>> Update(int id, [FromBody] LearningTrackUpdateRequest request, CancellationToken ct)
    {
        await _updateValidator.ValidateAndThrowAsync(request, ct);
        var result = await _service.UpdateAsync(id, request, ct);
        return Ok(ApiResponse<LearningTrackResponse>.Ok(result));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(new { id }));
    }

    [HttpPost("{id:int}/status")]
    public async Task<ActionResult<ApiResponse<LearningTrackResponse>>> ChangeStatus(int id, [FromBody] LearningTrackStatusUpdateRequest request, CancellationToken ct)
    {
        await _statusValidator.ValidateAndThrowAsync(request, ct);
        var result = await _service.UpdateStatusAsync(id, request, ct);
        return Ok(ApiResponse<LearningTrackResponse>.Ok(result));
    }

    [HttpPost("{id:int}/tags/{tagId:int}")]
    public async Task<ActionResult<ApiResponse<object>>> AttachTag(int id, int tagId, CancellationToken ct)
    {
        await _service.AttachTagAsync(id, tagId, ct);
        return Ok(ApiResponse<object>.Ok(new { learningTrackId = id, tagId }));
    }

    [HttpDelete("{id:int}/tags/{tagId:int}")]
    public async Task<ActionResult<ApiResponse<object>>> DetachTag(int id, int tagId, CancellationToken ct)
    {
        await _service.DetachTagAsync(id, tagId, ct);
        return Ok(ApiResponse<object>.Ok(new { learningTrackId = id, tagId }));
    }
}
