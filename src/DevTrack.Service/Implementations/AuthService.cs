using DevTrack.Domain.DTOs.Auth;
using DevTrack.Domain.Entities;
using DevTrack.Domain.Exceptions;
using DevTrack.Infrastructure.Auth;
using DevTrack.Repository.Interfaces;
using DevTrack.Service.Interfaces;

namespace DevTrack.Service.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _tokens;
    private readonly ICurrentUser _currentUser;

    public AuthService(IUserRepository users, IPasswordHasher hasher, IJwtTokenService tokens, ICurrentUser currentUser)
    {
        _users = users;
        _hasher = hasher;
        _tokens = tokens;
        _currentUser = currentUser;
    }

    public async Task<UserResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        if (await _users.UsernameExistsAsync(request.Username, ct))
            throw new ConflictException("Username is already taken.");
        if (await _users.EmailExistsAsync(request.Email, ct))
            throw new ConflictException("Email is already registered.");

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _hasher.Hash(request.Password)
        };

        await _users.AddAsync(user, ct);
        await _users.SaveChangesAsync(ct);

        return ToResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _users.GetByUsernameAsync(request.Username, ct);
        if (user is null || !_hasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAppException("Invalid credentials.");

        var (token, expires) = _tokens.GenerateAccessToken(user);

        return new AuthResponse
        {
            Token = token,
            ExpiresAt = expires,
            User = ToResponse(user)
        };
    }

    public async Task<UserResponse> GetCurrentUserAsync(CancellationToken ct = default)
    {
        var id = _currentUser.RequireUserId();
        var user = await _users.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("User not found.");
        return ToResponse(user);
    }

    private static UserResponse ToResponse(User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        CreatedAt = user.CreatedAt
    };
}
