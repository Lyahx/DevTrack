using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DevTrack.Domain.Exceptions;
using DevTrack.Service.Interfaces;

namespace DevTrack.Api.Auth;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _http;

    public CurrentUser(IHttpContextAccessor http)
    {
        _http = http;
    }

    public int? UserId
    {
        get
        {
            var principal = _http.HttpContext?.User;
            if (principal is null) return null;
            var sub = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(sub, out var id) ? id : null;
        }
    }

    public bool IsAuthenticated => _http.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public int RequireUserId() => UserId ?? throw new UnauthorizedAppException("Authentication required.");
}
