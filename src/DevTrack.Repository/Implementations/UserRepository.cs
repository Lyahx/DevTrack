using DevTrack.Domain.Entities;
using DevTrack.Infrastructure.Data;
using DevTrack.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Repository.Implementations;

public class UserRepository : IUserRepository
{
    private readonly DevTrackDbContext _db;

    public UserRepository(DevTrackDbContext db)
    {
        _db = db;
    }

    public Task<User?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
        => _db.Users.FirstOrDefaultAsync(u => u.Username == username, ct);

    public Task<bool> UsernameExistsAsync(string username, CancellationToken ct = default)
        => _db.Users.AnyAsync(u => u.Username == username, ct);

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
        => _db.Users.AnyAsync(u => u.Email == email, ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await _db.Users.AddAsync(user, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);

    public async Task<IReadOnlyList<User>> ListAllAsync(CancellationToken ct = default)
        => await _db.Users.ToListAsync(ct);
}
