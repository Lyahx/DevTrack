using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.Ideas;
using DevTrack.Domain.DTOs.QuickCapture;
using DevTrack.Service.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevTrack.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/quick-capture")]
public class QuickCaptureController : ControllerBase
{
    private readonly IQuickCaptureService _service;
    private readonly IValidator<QuickCaptureRequest> _validator;

    public QuickCaptureController(IQuickCaptureService service, IValidator<QuickCaptureRequest> validator)
    {
        _service = service;
        _validator = validator;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<IdeaResponse>>> Capture([FromBody] QuickCaptureRequest request, CancellationToken ct)
    {
        await _validator.ValidateAndThrowAsync(request, ct);
        var idea = await _service.CaptureAsync(request, ct);
        return Ok(ApiResponse<IdeaResponse>.Ok(idea));
    }
}
