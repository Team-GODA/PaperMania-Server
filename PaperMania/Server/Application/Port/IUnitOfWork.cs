using System.Data;
using Npgsql;

namespace Server.Application.Port;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IDbConnection Connection { get; }
    IDbTransaction Transaction { get; }
    
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}