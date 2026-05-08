using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.Resume;
using DevTrack.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevTrack.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1")]
public class ResumeController : ControllerBase
{
    private readonly IResumeService _service;

    public ResumeController(IResumeService service)
    {
        _service = service;
    }

    [HttpGet("projects/{id:int}/resume")]
    public async Task<ActionResult<ApiResponse<ResumeProjectResponse>>> ForProject(int id, CancellationToken ct)
        => Ok(ApiResponse<ResumeProjectResponse>.Ok(await _service.GetForProjectAsync(id, ct)));

    [HttpGet("components/{id:int}/resume")]
    public async Task<ActionResult<ApiResponse<ResumeComponentResponse>>> ForComponent(int id, CancellationToken ct)
        => Ok(ApiResponse<ResumeComponentResponse>.Ok(await _service.GetForComponentAsync(id, ct)));

    [HttpGet("learning-tracks/{id:int}/resume")]
    public async Task<ActionResult<ApiResponse<ResumeLearningTrackResponse>>> ForLearningTrack(int id, CancellationToken ct)
        => Ok(ApiResponse<ResumeLearningTrackResponse>.Ok(await _service.GetForLearningTrackAsync(id, ct)));

    [HttpGet("modules/{id:int}/resume")]
    public async Task<ActionResult<ApiResponse<ResumeLearningModuleResponse>>> ForLearningModule(int id, CancellationToken ct)
        => Ok(ApiResponse<ResumeLearningModuleResponse>.Ok(await _service.GetForLearningModuleAsync(id, ct)));
}
