using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.LearningTracks;
using DevTrack.Service.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevTrack.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1")]
public class LearningModulesController : ControllerBase
{
    private readonly ILearningModuleService _service;
    private readonly IValidator<LearningModuleCreateRequest> _createValidator;
    private readonly IValidator<LearningModuleUpdateRequest> _updateValidator;
    private readonly IValidator<LearningModuleStatusUpdateRequest> _statusValidator;
    private readonly IValidator<LearningModuleOrderUpdateRequest> _orderValidator;

    public LearningModulesController(
        ILearningModuleService service,
        IValidator<LearningModuleCreateRequest> createValidator,
        IValidator<LearningModuleUpdateRequest> updateValidator,
        IValidator<LearningModuleStatusUpdateRequest> statusValidator,
        IValidator<LearningModuleOrderUpdateRequest> orderValidator)
    {
        _service = service;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _statusValidator = statusValidator;
        _orderValidator = orderValidator;
    }

    [HttpGet("learning-tracks/{trackId:int}/modules")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<LearningModuleResponse>>>> List(
        int trackId,
        [FromQuery] bool includeDeleted = false,
        CancellationToken ct = default)
    {
        var items = await _service.ListByTrackAsync(trackId, includeDeleted, ct);
        return Ok(ApiResponse<IReadOnlyList<LearningModuleResponse>>.Ok(items));
    }

    [HttpGet("modules/{id:int}")]
    public async Task<ActionResult<ApiResponse<LearningModuleResponse>>> Get(int id, [FromQuery] bool includeDeleted = false, CancellationToken ct = default)
    {
        var item = await _service.GetAsync(id, includeDeleted, ct);
        return Ok(ApiResponse<LearningModuleResponse>.Ok(item));
    }

    [HttpPost("learning-tracks/{trackId:int}/modules")]
    public async Task<ActionResult<ApiResponse<LearningModuleResponse>>> Create(int trackId, [FromBody] LearningModuleCreateRequest request, CancellationToken ct)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);
        var item = await _service.CreateAsync(trackId, request, ct);
        return CreatedAtAction(nameof(Get), new { id = item.Id }, ApiResponse<LearningModuleResponse>.Ok(item));
    }

    [HttpPut("modules/{id:int}")]
    public async Task<ActionResult<ApiResponse<LearningModuleResponse>>> Update(int id, [FromBody] LearningModuleUpdateRequest request, CancellationToken ct)
    {
        await _updateValidator.ValidateAndThrowAsync(request, ct);
        var item = await _service.UpdateAsync(id, request, ct);
        return Ok(ApiResponse<LearningModuleResponse>.Ok(item));
    }

    [HttpDelete("modules/{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(new { id }));
    }

    [HttpPut("modules/{id:int}/status")]
    public async Task<ActionResult<ApiResponse<LearningModuleResponse>>> UpdateStatus(int id, [FromBody] LearningModuleStatusUpdateRequest request, CancellationToken ct)
    {
        await _statusValidator.ValidateAndThrowAsync(request, ct);
        var item = await _service.UpdateStatusAsync(id, request, ct);
        return Ok(ApiResponse<LearningModuleResponse>.Ok(item));
    }

    [HttpPut("modules/{id:int}/order")]
    public async Task<ActionResult<ApiResponse<LearningModuleResponse>>> UpdateOrder(int id, [FromBody] LearningModuleOrderUpdateRequest request, CancellationToken ct)
    {
        await _orderValidator.ValidateAndThrowAsync(request, ct);
        var item = await _service.UpdateOrderAsync(id, request, ct);
        return Ok(ApiResponse<LearningModuleResponse>.Ok(item));
    }
}
