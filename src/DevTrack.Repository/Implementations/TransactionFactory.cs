using DevTrack.Infrastructure.Data;
using DevTrack.Repository.Common;
using Microsoft.EntityFrameworkCore.Storage;

namespace DevTrack.Repository.Implementations;

public class TransactionFactory : ITransactionFactory
{
    private readonly DevTrackDbContext _db;

    public TransactionFactory(DevTrackDbContext db)
    {
        _db = db;
    }

    public async Task<ITransactionScope> BeginAsync(CancellationToken ct = default)
        => new EfTransactionScope(await _db.Database.BeginTransactionAsync(ct));

    private sealed class EfTransactionScope : ITransactionScope
    {
        private readonly IDbContextTransaction _tx;
        public EfTransactionScope(IDbContextTransaction tx) => _tx = tx;
        public Task CommitAsync(CancellationToken ct = default) => _tx.CommitAsync(ct);
        public Task RollbackAsync(CancellationToken ct = default) => _tx.RollbackAsync(ct);
        public ValueTask DisposeAsync() => _tx.DisposeAsync();
    }
}
