using System.Data;
using Npgsql;

namespace Server.Application.Port;

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