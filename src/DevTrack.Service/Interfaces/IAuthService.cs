using DevTrack.Domain.DTOs.Auth;

namespace DevTrack.Service.Interfaces;

public interface IAuthService
{
    Task<UserResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<UserResponse> GetCurrentUserAsync(CancellationToken ct = default);
}
