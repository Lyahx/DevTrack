namespace DevTrack.Service.Interfaces;

public interface ICurrentUser
{
    int? UserId { get; }
    bool IsAuthenticated { get; }
    int RequireUserId();
}
