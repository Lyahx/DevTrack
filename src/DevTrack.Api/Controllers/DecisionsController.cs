using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.Decisions;
using DevTrack.Service.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevTrack.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1")]
public class DecisionsController : ControllerBase
{
    private readonly IDecisionService _service;
    private readonly IValidator<DecisionCreateRequest> _createValidator;
    private readonly IValidator<DecisionUpdateRequest> _updateValidator;

    public DecisionsController(
        IDecisionService service,
        IValidator<DecisionCreateRequest> createValidator,
        IValidator<DecisionUpdateRequest> updateValidator)
    {
        _service = service;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet("decisions")]
    public async Task<ActionResult<ApiResponse<PagedResult<DecisionResponse>>>> List([FromQuery] DecisionListQuery query, CancellationToken ct)
        => Ok(ApiResponse<PagedResult<DecisionResponse>>.Ok(await _service.ListAsync(query, ct)));

    [HttpGet("decisions/{id:int}")]
    public async Task<ActionResult<ApiResponse<DecisionResponse>>> Get(int id, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
        => Ok(ApiResponse<DecisionResponse>.Ok(await _service.GetAsync(id, includeDeleted, ct)));

    [HttpPost("decisions")]
    public async Task<ActionResult<ApiResponse<DecisionResponse>>> Create([FromBody] DecisionCreateRequest request, CancellationToken ct)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);
        var item = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = item.Id }, ApiResponse<DecisionResponse>.Ok(item));
    }

    [HttpPut("decisions/{id:int}")]
    public async Task<ActionResult<ApiResponse<DecisionResponse>>> Update(int id, [FromBody] DecisionUpdateRequest request, CancellationToken ct)
    {
        await _updateValidator.ValidateAndThrowAsync(request, ct);
        var item = await _service.UpdateAsync(id, request, ct);
        return Ok(ApiResponse<DecisionResponse>.Ok(item));
    }

    [HttpDelete("decisions/{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(new { id }));
    }

    [HttpGet("projects/{projectId:int}/decisions")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DecisionResponse>>>> ListForProject(int projectId, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
        => Ok(ApiResponse<IReadOnlyList<DecisionResponse>>.Ok(await _service.ListForProjectAsync(projectId, includeDeleted, ct)));

    [HttpGet("components/{componentId:int}/decisions")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DecisionResponse>>>> ListForComponent(int componentId, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
        => Ok(ApiResponse<IReadOnlyList<DecisionResponse>>.Ok(await _service.ListForComponentAsync(componentId, includeDeleted, ct)));

    [HttpGet("learning-tracks/{trackId:int}/decisions")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DecisionResponse>>>> ListForTrack(int trackId, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
        => Ok(ApiResponse<IReadOnlyList<DecisionResponse>>.Ok(await _service.ListForLearningTrackAsync(trackId, includeDeleted, ct)));

    [HttpGet("modules/{moduleId:int}/decisions")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DecisionResponse>>>> ListForModule(int moduleId, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
        => Ok(ApiResponse<IReadOnlyList<DecisionResponse>>.Ok(await _service.ListForLearningModuleAsync(moduleId, includeDeleted, ct)));
}
