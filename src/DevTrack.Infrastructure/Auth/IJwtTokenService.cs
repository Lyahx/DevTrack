using DevTrack.Domain.Entities;

namespace DevTrack.Infrastructure.Auth;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) GenerateAccessToken(User user);
}
