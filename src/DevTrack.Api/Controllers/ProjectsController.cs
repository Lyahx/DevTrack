using DevTrack.Domain.DTOs.Commits;
using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.Projects;
using DevTrack.Service.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevTrack.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/projects")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _service;
    private readonly IValidator<ProjectCreateRequest> _createValidator;
    private readonly IValidator<ProjectUpdateRequest> _updateValidator;
    private readonly IValidator<ProjectStatusUpdateRequest> _statusValidator;

    public ProjectsController(
        IProjectService service,
        IValidator<ProjectCreateRequest> createValidator,
        IValidator<ProjectUpdateRequest> updateValidator,
        IValidator<ProjectStatusUpdateRequest> statusValidator)
    {
        _service = service;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _statusValidator = statusValidator;
    }

    [HttpGet("{id:int}/commits")]
    public async Task<ActionResult<ApiResponse<CommitListResponse>>> Commits(
        int id,
        [FromServices] ICommitService commits,
        [FromQuery] int limit = 10,
        CancellationToken ct = default)
    {
        var clamped = Math.Clamp(limit, 1, 30);
        var result = await commits.GetForProjectAsync(id, clamped, ct);
        return Ok(ApiResponse<CommitListResponse>.Ok(result));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ProjectResponse>>>> List(
        [FromQuery] ProjectListQuery query,
        CancellationToken ct)
    {
        var result = await _service.ListAsync(query, ct);
        return Ok(ApiResponse<PagedResult<ProjectResponse>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ProjectResponse>>> Get(int id, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
    {
        var result = await _service.GetAsync(id, includeDeleted, ct);
        return Ok(ApiResponse<ProjectResponse>.Ok(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProjectResponse>>> Create([FromBody] ProjectCreateRequest request, CancellationToken ct)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);
        var result = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, ApiResponse<ProjectResponse>.Ok(result));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<ProjectResponse>>> Update(int id, [FromBody] ProjectUpdateRequest request, CancellationToken ct)
    {
        await _updateValidator.ValidateAndThrowAsync(request, ct);
        var result = await _service.UpdateAsync(id, request, ct);
        return Ok(ApiResponse<ProjectResponse>.Ok(result));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(new { id }));
    }

    [HttpPost("{id:int}/status")]
    public async Task<ActionResult<ApiResponse<ProjectResponse>>> ChangeStatus(int id, [FromBody] ProjectStatusUpdateRequest request, CancellationToken ct)
    {
        await _statusValidator.ValidateAndThrowAsync(request, ct);
        var result = await _service.UpdateStatusAsync(id, request, ct);
        return Ok(ApiResponse<ProjectResponse>.Ok(result));
    }

    [HttpPost("{id:int}/tags/{tagId:int}")]
    public async Task<ActionResult<ApiResponse<object>>> AttachTag(int id, int tagId, CancellationToken ct)
    {
        await _service.AttachTagAsync(id, tagId, ct);
        return Ok(ApiResponse<object>.Ok(new { projectId = id, tagId }));
    }

    [HttpDelete("{id:int}/tags/{tagId:int}")]
    public async Task<ActionResult<ApiResponse<object>>> DetachTag(int id, int tagId, CancellationToken ct)
    {
        await _service.DetachTagAsync(id, tagId, ct);
        return Ok(ApiResponse<object>.Ok(new { projectId = id, tagId }));
    }
}
