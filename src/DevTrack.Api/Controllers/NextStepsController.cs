using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.NextSteps;
using DevTrack.Service.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevTrack.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1")]
public class NextStepsController : ControllerBase
{
    private readonly INextStepService _service;
    private readonly IValidator<NextStepCreateRequest> _createValidator;
    private readonly IValidator<NextStepUpdateRequest> _updateValidator;

    public NextStepsController(
        INextStepService service,
        IValidator<NextStepCreateRequest> createValidator,
        IValidator<NextStepUpdateRequest> updateValidator)
    {
        _service = service;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet("next-steps")]
    public async Task<ActionResult<ApiResponse<PagedResult<NextStepResponse>>>> List([FromQuery] NextStepListQuery query, CancellationToken ct)
        => Ok(ApiResponse<PagedResult<NextStepResponse>>.Ok(await _service.ListAsync(query, ct)));

    [HttpGet("next-steps/open")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<NextStepResponse>>>> ListOpen(CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<NextStepResponse>>.Ok(await _service.ListOpenAsync(ct)));

    [HttpGet("next-steps/{id:int}")]
    public async Task<ActionResult<ApiResponse<NextStepResponse>>> Get(int id, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
        => Ok(ApiResponse<NextStepResponse>.Ok(await _service.GetAsync(id, includeDeleted, ct)));

    [HttpPost("next-steps")]
    public async Task<ActionResult<ApiResponse<NextStepResponse>>> Create([FromBody] NextStepCreateRequest request, CancellationToken ct)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);
        var item = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = item.Id }, ApiResponse<NextStepResponse>.Ok(item));
    }

    [HttpPut("next-steps/{id:int}")]
    public async Task<ActionResult<ApiResponse<NextStepResponse>>> Update(int id, [FromBody] NextStepUpdateRequest request, CancellationToken ct)
    {
        await _updateValidator.ValidateAndThrowAsync(request, ct);
        var item = await _service.UpdateAsync(id, request, ct);
        return Ok(ApiResponse<NextStepResponse>.Ok(item));
    }

    [HttpDelete("next-steps/{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(new { id }));
    }

    [HttpPut("next-steps/{id:int}/complete")]
    public async Task<ActionResult<ApiResponse<NextStepResponse>>> Complete(int id, CancellationToken ct)
        => Ok(ApiResponse<NextStepResponse>.Ok(await _service.CompleteAsync(id, ct)));

    [HttpPut("next-steps/{id:int}/uncomplete")]
    public async Task<ActionResult<ApiResponse<NextStepResponse>>> Uncomplete(int id, CancellationToken ct)
        => Ok(ApiResponse<NextStepResponse>.Ok(await _service.UncompleteAsync(id, ct)));

    [HttpGet("projects/{projectId:int}/next-steps")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<NextStepResponse>>>> ListForProject(int projectId, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
        => Ok(ApiResponse<IReadOnlyList<NextStepResponse>>.Ok(await _service.ListForProjectAsync(projectId, includeDeleted, ct)));

    [HttpGet("components/{componentId:int}/next-steps")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<NextStepResponse>>>> ListForComponent(int componentId, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
        => Ok(ApiResponse<IReadOnlyList<NextStepResponse>>.Ok(await _service.ListForComponentAsync(componentId, includeDeleted, ct)));

    [HttpGet("learning-tracks/{trackId:int}/next-steps")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<NextStepResponse>>>> ListForTrack(int trackId, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
        => Ok(ApiResponse<IReadOnlyList<NextStepResponse>>.Ok(await _service.ListForLearningTrackAsync(trackId, includeDeleted, ct)));

    [HttpGet("modules/{moduleId:int}/next-steps")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<NextStepResponse>>>> ListForModule(int moduleId, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
        => Ok(ApiResponse<IReadOnlyList<NextStepResponse>>.Ok(await _service.ListForLearningModuleAsync(moduleId, includeDeleted, ct)));
}
