using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.Dashboard;
using DevTrack.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevTrack.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<DashboardResponse>>> Get(CancellationToken ct)
        => Ok(ApiResponse<DashboardResponse>.Ok(await _service.GetAsync(ct)));
}
