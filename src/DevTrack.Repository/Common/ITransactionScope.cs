namespace DevTrack.Repository.Common;

public interface ITransactionScope : IAsyncDisposable
{
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
}

public interface ITransactionFactory
{
    Task<ITransactionScope> BeginAsync(CancellationToken ct = default);
}
