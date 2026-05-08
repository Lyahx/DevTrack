using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.Ideas;
using DevTrack.Domain.DTOs.NextSteps;
using DevTrack.Service.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevTrack.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1")]
public class IdeasController : ControllerBase
{
    private readonly IIdeaService _service;
    private readonly IValidator<IdeaCreateRequest> _createValidator;
    private readonly IValidator<IdeaUpdateRequest> _updateValidator;
    private readonly IValidator<IdeaConvertRequest> _convertValidator;

    public IdeasController(
        IIdeaService service,
        IValidator<IdeaCreateRequest> createValidator,
        IValidator<IdeaUpdateRequest> updateValidator,
        IValidator<IdeaConvertRequest> convertValidator)
    {
        _service = service;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _convertValidator = convertValidator;
    }

    [HttpGet("ideas")]
    public async Task<ActionResult<ApiResponse<PagedResult<IdeaResponse>>>> List([FromQuery] IdeaListQuery query, CancellationToken ct)
        => Ok(ApiResponse<PagedResult<IdeaResponse>>.Ok(await _service.ListAsync(query, ct)));

    [HttpGet("ideas/{id:int}")]
    public async Task<ActionResult<ApiResponse<IdeaResponse>>> Get(int id, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
        => Ok(ApiResponse<IdeaResponse>.Ok(await _service.GetAsync(id, includeDeleted, ct)));

    [HttpPost("ideas")]
    public async Task<ActionResult<ApiResponse<IdeaResponse>>> Create([FromBody] IdeaCreateRequest request, CancellationToken ct)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);
        var item = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = item.Id }, ApiResponse<IdeaResponse>.Ok(item));
    }

    [HttpPut("ideas/{id:int}")]
    public async Task<ActionResult<ApiResponse<IdeaResponse>>> Update(int id, [FromBody] IdeaUpdateRequest request, CancellationToken ct)
    {
        await _updateValidator.ValidateAndThrowAsync(request, ct);
        var item = await _service.UpdateAsync(id, request, ct);
        return Ok(ApiResponse<IdeaResponse>.Ok(item));
    }

    [HttpDelete("ideas/{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(new { id }));
    }

    [HttpPost("ideas/{id:int}/convert-to-next-step")]
    public async Task<ActionResult<ApiResponse<NextStepResponse>>> Convert(int id, [FromBody] IdeaConvertRequest request, CancellationToken ct)
    {
        await _convertValidator.ValidateAndThrowAsync(request, ct);
        var step = await _service.ConvertToNextStepAsync(id, request, ct);
        return Ok(ApiResponse<NextStepResponse>.Ok(step));
    }

    [HttpGet("projects/{projectId:int}/ideas")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<IdeaResponse>>>> ListForProject(int projectId, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
        => Ok(ApiResponse<IReadOnlyList<IdeaResponse>>.Ok(await _service.ListForProjectAsync(projectId, includeDeleted, ct)));

    [HttpGet("components/{componentId:int}/ideas")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<IdeaResponse>>>> ListForComponent(int componentId, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
        => Ok(ApiResponse<IReadOnlyList<IdeaResponse>>.Ok(await _service.ListForComponentAsync(componentId, includeDeleted, ct)));

    [HttpGet("learning-tracks/{trackId:int}/ideas")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<IdeaResponse>>>> ListForTrack(int trackId, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
        => Ok(ApiResponse<IReadOnlyList<IdeaResponse>>.Ok(await _service.ListForLearningTrackAsync(trackId, includeDeleted, ct)));

    [HttpGet("modules/{moduleId:int}/ideas")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<IdeaResponse>>>> ListForModule(int moduleId, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
        => Ok(ApiResponse<IReadOnlyList<IdeaResponse>>.Ok(await _service.ListForLearningModuleAsync(moduleId, includeDeleted, ct)));
}
