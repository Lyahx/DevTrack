using DevTrack.Domain.DTOs.Auth;
using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.Entities;
using DevTrack.Infrastructure.Auth;
using DevTrack.Repository.Interfaces;
using DevTrack.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevTrack.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/dev")]
public class DevController : ControllerBase
{
    [HttpPost("quick-login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> QuickLogin(
        [FromServices] IUserRepository users,
        [FromServices] IPasswordHasher hasher,
        [FromServices] IJwtTokenService tokens,
        [FromServices] IWebHostEnvironment env,
        CancellationToken ct)
    {
        if (!env.IsDevelopment())
            return NotFound(ApiResponse<AuthResponse>.Fail("NOT_FOUND", "Dev endpoints only run in Development."));

        const string username = "dev";
        const string password = "devpass123";
        const string email = "dev@devtrack.local";

        var user = await users.GetByUsernameAsync(username, ct);
        if (user is null)
        {
            user = new User { Username = username, Email = email, PasswordHash = hasher.Hash(password) };
            await users.AddAsync(user, ct);
            await users.SaveChangesAsync(ct);
        }

        var (token, expires) = tokens.GenerateAccessToken(user);
        return Ok(ApiResponse<AuthResponse>.Ok(new AuthResponse
        {
            Token = token,
            ExpiresAt = expires,
            User = new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
            },
        }));
    }

    [HttpPost("seed")]
    public async Task<ActionResult<ApiResponse<DevSeedResult>>> Seed(
        [FromServices] IDevSeedService? seed,
        [FromServices] ICurrentUser currentUser,
        [FromServices] IWebHostEnvironment env,
        CancellationToken ct)
    {
        if (seed is null || !env.IsDevelopment())
            return NotFound(ApiResponse<DevSeedResult>.Fail("NOT_FOUND", "Dev endpoints only run in Development."));

        var userId = currentUser.RequireUserId();
        var result = await seed.SeedAsync(userId, ct);
        return Ok(ApiResponse<DevSeedResult>.Ok(result));
    }

    [HttpPost("wipe")]
    public async Task<ActionResult<ApiResponse<object>>> Wipe(
        [FromServices] IDevSeedService? seed,
        [FromServices] ICurrentUser currentUser,
        [FromServices] IWebHostEnvironment env,
        CancellationToken ct)
    {
        if (seed is null || !env.IsDevelopment())
            return NotFound(ApiResponse<object>.Fail("NOT_FOUND", "Dev endpoints only run in Development."));

        var userId = currentUser.RequireUserId();
        var rows = await seed.WipeAsync(userId, ct);
        return Ok(ApiResponse<object>.Ok(new { rowsDeleted = rows }));
    }
}
