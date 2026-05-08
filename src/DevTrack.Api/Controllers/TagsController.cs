using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.Tags;
using DevTrack.Service.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevTrack.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/tags")]
public class TagsController : ControllerBase
{
    private readonly ITagService _service;
    private readonly IValidator<TagCreateRequest> _createValidator;
    private readonly IValidator<TagUpdateRequest> _updateValidator;

    public TagsController(
        ITagService service,
        IValidator<TagCreateRequest> createValidator,
        IValidator<TagUpdateRequest> updateValidator)
    {
        _service = service;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TagResponse>>>> List(CancellationToken ct)
    {
        var items = await _service.ListAsync(ct);
        return Ok(ApiResponse<IReadOnlyList<TagResponse>>.Ok(items));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TagResponse>>> Create([FromBody] TagCreateRequest request, CancellationToken ct)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);
        var item = await _service.CreateAsync(request, ct);
        return Ok(ApiResponse<TagResponse>.Ok(item));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<TagResponse>>> Update(int id, [FromBody] TagUpdateRequest request, CancellationToken ct)
    {
        await _updateValidator.ValidateAndThrowAsync(request, ct);
        var item = await _service.UpdateAsync(id, request, ct);
        return Ok(ApiResponse<TagResponse>.Ok(item));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(new { id }));
    }
}
