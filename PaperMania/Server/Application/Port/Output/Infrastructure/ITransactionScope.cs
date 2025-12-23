using System.Data;

namespace Server.Application.Port.Output.Infrastructure;

public interface ITransactionScope : IDisposable, IAsyncDisposable
{
    IDbConnection Connection { get; }
    IDbTransaction Transaction { get; }

    Task BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    Task CommitAsync();
    Task RollbackAsync();

    Task ExecuteAsync(Func<Task> action);
    Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action);
}