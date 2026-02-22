using System.Data;

namespace Server.Application.Port.Output.Transaction;

public interface ITransactionScope : IDisposable, IAsyncDisposable
{
    IDbConnection Connection { get; }
    IDbTransaction Transaction { get; }

    Task BeginTransactionAsync(
        IsolationLevel isolationLevel,
        CancellationToken ct);
    Task CommitAsync(CancellationToken ct);
    Task RollbackAsync(CancellationToken ct);

    Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken ct);
    Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> action, CancellationToken ct);
}