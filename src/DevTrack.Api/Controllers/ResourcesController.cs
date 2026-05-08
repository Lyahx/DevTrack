using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.Resources;
using DevTrack.Service.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevTrack.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1")]
public class ResourcesController : ControllerBase
{
    private readonly IResourceService _service;
    private readonly IValidator<ResourceCreateRequest> _createValidator;
    private readonly IValidator<ResourceUpdateRequest> _updateValidator;

    public ResourcesController(
        IResourceService service,
        IValidator<ResourceCreateRequest> createValidator,
        IValidator<ResourceUpdateRequest> updateValidator)
    {
        _service = service;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet("resources")]
    public async Task<ActionResult<ApiResponse<PagedResult<ResourceResponse>>>> List([FromQuery] ResourceListQuery query, CancellationToken ct)
        => Ok(ApiResponse<PagedResult<ResourceResponse>>.Ok(await _service.ListAsync(query, ct)));

    [HttpGet("resources/{id:int}")]
    public async Task<ActionResult<ApiResponse<ResourceResponse>>> Get(int id, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
        => Ok(ApiResponse<ResourceResponse>.Ok(await _service.GetAsync(id, includeDeleted, ct)));

    [HttpPost("resources")]
    public async Task<ActionResult<ApiResponse<ResourceResponse>>> Create([FromBody] ResourceCreateRequest request, CancellationToken ct)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);
        var item = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = item.Id }, ApiResponse<ResourceResponse>.Ok(item));
    }

    [HttpPut("resources/{id:int}")]
    public async Task<ActionResult<ApiResponse<ResourceResponse>>> Update(int id, [FromBody] ResourceUpdateRequest request, CancellationToken ct)
    {
        await _updateValidator.ValidateAndThrowAsync(request, ct);
        var item = await _service.UpdateAsync(id, request, ct);
        return Ok(ApiResponse<ResourceResponse>.Ok(item));
    }

    [HttpDelete("resources/{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(new { id }));
    }

    [HttpGet("projects/{projectId:int}/resources")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ResourceResponse>>>> ListForProject(int projectId, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
        => Ok(ApiResponse<IReadOnlyList<ResourceResponse>>.Ok(await _service.ListForProjectAsync(projectId, includeDeleted, ct)));

    [HttpGet("components/{componentId:int}/resources")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ResourceResponse>>>> ListForComponent(int componentId, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
        => Ok(ApiResponse<IReadOnlyList<ResourceResponse>>.Ok(await _service.ListForComponentAsync(componentId, includeDeleted, ct)));

    [HttpGet("learning-tracks/{trackId:int}/resources")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ResourceResponse>>>> ListForTrack(int trackId, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
        => Ok(ApiResponse<IReadOnlyList<ResourceResponse>>.Ok(await _service.ListForLearningTrackAsync(trackId, includeDeleted, ct)));

    [HttpGet("modules/{moduleId:int}/resources")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ResourceResponse>>>> ListForModule(int moduleId, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
        => Ok(ApiResponse<IReadOnlyList<ResourceResponse>>.Ok(await _service.ListForLearningModuleAsync(moduleId, includeDeleted, ct)));
}
