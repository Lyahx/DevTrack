using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.Worklogs;
using DevTrack.Service.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevTrack.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1")]
public class WorklogsController : ControllerBase
{
    private readonly IWorklogService _service;
    private readonly IValidator<WorklogCreateRequest> _createValidator;
    private readonly IValidator<WorklogUpdateRequest> _updateValidator;

    public WorklogsController(
        IWorklogService service,
        IValidator<WorklogCreateRequest> createValidator,
        IValidator<WorklogUpdateRequest> updateValidator)
    {
        _service = service;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet("worklogs")]
    public async Task<ActionResult<ApiResponse<PagedResult<WorklogResponse>>>> List([FromQuery] WorklogListQuery query, CancellationToken ct)
    {
        var result = await _service.ListAsync(query, ct);
        return Ok(ApiResponse<PagedResult<WorklogResponse>>.Ok(result));
    }

    [HttpGet("worklogs/recent")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<WorklogResponse>>>> Recent([FromQuery] int days = 7, CancellationToken ct = default)
    {
        var items = await _service.ListRecentAsync(Math.Clamp(days, 1, 90), ct);
        return Ok(ApiResponse<IReadOnlyList<WorklogResponse>>.Ok(items));
    }

    [HttpGet("worklogs/{id:int}")]
    public async Task<ActionResult<ApiResponse<WorklogResponse>>> Get(int id, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
    {
        var item = await _service.GetAsync(id, includeDeleted, ct);
        return Ok(ApiResponse<WorklogResponse>.Ok(item));
    }

    [HttpPost("worklogs")]
    public async Task<ActionResult<ApiResponse<WorklogResponse>>> Create([FromBody] WorklogCreateRequest request, CancellationToken ct)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);
        var item = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = item.Id }, ApiResponse<WorklogResponse>.Ok(item));
    }

    [HttpPut("worklogs/{id:int}")]
    public async Task<ActionResult<ApiResponse<WorklogResponse>>> Update(int id, [FromBody] WorklogUpdateRequest request, CancellationToken ct)
    {
        await _updateValidator.ValidateAndThrowAsync(request, ct);
        var item = await _service.UpdateAsync(id, request, ct);
        return Ok(ApiResponse<WorklogResponse>.Ok(item));
    }

    [HttpDelete("worklogs/{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(new { id }));
    }

    [HttpGet("projects/{projectId:int}/worklogs")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<WorklogResponse>>>> ListForProject(int projectId, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
        => Ok(ApiResponse<IReadOnlyList<WorklogResponse>>.Ok(await _service.ListForProjectAsync(projectId, includeDeleted, ct)));

    [HttpGet("components/{componentId:int}/worklogs")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<WorklogResponse>>>> ListForComponent(int componentId, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
        => Ok(ApiResponse<IReadOnlyList<WorklogResponse>>.Ok(await _service.ListForComponentAsync(componentId, includeDeleted, ct)));

    [HttpGet("learning-tracks/{trackId:int}/worklogs")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<WorklogResponse>>>> ListForTrack(int trackId, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
        => Ok(ApiResponse<IReadOnlyList<WorklogResponse>>.Ok(await _service.ListForLearningTrackAsync(trackId, includeDeleted, ct)));

    [HttpGet("modules/{moduleId:int}/worklogs")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<WorklogResponse>>>> ListForModule(int moduleId, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
        => Ok(ApiResponse<IReadOnlyList<WorklogResponse>>.Ok(await _service.ListForLearningModuleAsync(moduleId, includeDeleted, ct)));
}
