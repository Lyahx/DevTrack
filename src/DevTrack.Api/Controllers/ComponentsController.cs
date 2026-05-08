using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.Components;
using DevTrack.Service.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevTrack.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1")]
public class ComponentsController : ControllerBase
{
    private readonly IComponentService _service;
    private readonly IValidator<ComponentCreateRequest> _createValidator;
    private readonly IValidator<ComponentUpdateRequest> _updateValidator;
    private readonly IValidator<ComponentStatusNoteRequest> _statusNoteValidator;

    public ComponentsController(
        IComponentService service,
        IValidator<ComponentCreateRequest> createValidator,
        IValidator<ComponentUpdateRequest> updateValidator,
        IValidator<ComponentStatusNoteRequest> statusNoteValidator)
    {
        _service = service;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _statusNoteValidator = statusNoteValidator;
    }

    [HttpGet("projects/{projectId:int}/components")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ComponentResponse>>>> List(
        int projectId,
        [FromQuery] bool includeDeleted = false,
        CancellationToken ct = default)
    {
        var items = await _service.ListByProjectAsync(projectId, includeDeleted, ct);
        return Ok(ApiResponse<IReadOnlyList<ComponentResponse>>.Ok(items));
    }

    [HttpGet("components/{id:int}")]
    public async Task<ActionResult<ApiResponse<ComponentResponse>>> Get(int id, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
    {
        var item = await _service.GetAsync(id, includeDeleted, ct);
        return Ok(ApiResponse<ComponentResponse>.Ok(item));
    }

    [HttpPost("projects/{projectId:int}/components")]
    public async Task<ActionResult<ApiResponse<ComponentResponse>>> Create(int projectId, [FromBody] ComponentCreateRequest request, CancellationToken ct)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);
        var item = await _service.CreateAsync(projectId, request, ct);
        return CreatedAtAction(nameof(Get), new { id = item.Id }, ApiResponse<ComponentResponse>.Ok(item));
    }

    [HttpPut("components/{id:int}")]
    public async Task<ActionResult<ApiResponse<ComponentResponse>>> Update(int id, [FromBody] ComponentUpdateRequest request, CancellationToken ct)
    {
        await _updateValidator.ValidateAndThrowAsync(request, ct);
        var item = await _service.UpdateAsync(id, request, ct);
        return Ok(ApiResponse<ComponentResponse>.Ok(item));
    }

    [HttpDelete("components/{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(new { id }));
    }

    [HttpPut("components/{id:int}/status-note")]
    public async Task<ActionResult<ApiResponse<ComponentResponse>>> UpdateStatusNote(int id, [FromBody] ComponentStatusNoteRequest request, CancellationToken ct)
    {
        await _statusNoteValidator.ValidateAndThrowAsync(request, ct);
        var item = await _service.UpdateStatusNoteAsync(id, request, ct);
        return Ok(ApiResponse<ComponentResponse>.Ok(item));
    }

    [HttpPost("components/{id:int}/tags/{tagId:int}")]
    public async Task<ActionResult<ApiResponse<object>>> AttachTag(int id, int tagId, CancellationToken ct)
    {
        await _service.AttachTagAsync(id, tagId, ct);
        return Ok(ApiResponse<object>.Ok(new { componentId = id, tagId }));
    }

    [HttpDelete("components/{id:int}/tags/{tagId:int}")]
    public async Task<ActionResult<ApiResponse<object>>> DetachTag(int id, int tagId, CancellationToken ct)
    {
        await _service.DetachTagAsync(id, tagId, ct);
        return Ok(ApiResponse<object>.Ok(new { componentId = id, tagId }));
    }
}
