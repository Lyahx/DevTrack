using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.Reminders;
using DevTrack.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevTrack.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/reminders")]
public class RemindersController : ControllerBase
{
    private readonly IReminderService _service;
    private readonly IReminderGenerator _generator;

    public RemindersController(IReminderService service, IReminderGenerator generator)
    {
        _service = service;
        _generator = generator;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ReminderResponse>>>> List([FromQuery] ReminderListQuery query, CancellationToken ct)
        => Ok(ApiResponse<PagedResult<ReminderResponse>>.Ok(await _service.ListAsync(query, ct)));

    [HttpGet("unread")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ReminderResponse>>>> Unread(CancellationToken ct)
        => Ok(ApiResponse<IReadOnlyList<ReminderResponse>>.Ok(await _service.ListUnreadAsync(ct)));

    [HttpPut("{id:int}/read")]
    public async Task<ActionResult<ApiResponse<ReminderResponse>>> MarkRead(int id, CancellationToken ct)
        => Ok(ApiResponse<ReminderResponse>.Ok(await _service.MarkReadAsync(id, ct)));

    [HttpPut("{id:int}/dismiss")]
    public async Task<ActionResult<ApiResponse<ReminderResponse>>> Dismiss(int id, CancellationToken ct)
        => Ok(ApiResponse<ReminderResponse>.Ok(await _service.DismissAsync(id, ct)));

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(new { id }));
    }

    [HttpPost("run-generator")]
    public async Task<ActionResult<ApiResponse<object>>> RunGenerator(CancellationToken ct)
    {
        var count = await _generator.GenerateForAllUsersAsync(ct);
        return Ok(ApiResponse<object>.Ok(new { generated = count }));
    }
}
