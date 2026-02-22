using Npgsql;
using Server.Application.Port.Output.Transaction;

namespace Server.Infrastructure.Persistence.Dao;

public class RepositoryBase
{ 
    private readonly string _connectionString;
    private readonly ITransactionScope? _transactionScope;

    protected RepositoryBase(
        string connectionString,
        ITransactionScope? transactionScope)
    {
        _connectionString = connectionString;
        _transactionScope = transactionScope;
    }

    protected async Task<T> ExecuteAsync<T>(
        Func<NpgsqlConnection, NpgsqlTransaction?, Task<T>> query,
        CancellationToken ct)
    {
        if (_transactionScope != null)
        {
            var connection = (NpgsqlConnection)_transactionScope.Connection;
            var transaction = _transactionScope.Transaction as NpgsqlTransaction;
            return await query(connection, transaction);
        }

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        return await query(conn, null);
    }
    
    protected async Task ExecuteAsync(
        Func<NpgsqlConnection, NpgsqlTransaction?, Task> query,
        CancellationToken ct)
    {
        if (_transactionScope != null)
        {
            var connection = (NpgsqlConnection)_transactionScope.Connection;
            var transaction = _transactionScope.Transaction as NpgsqlTransaction;
            await query(connection, transaction);
            return;
        }

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        await query(conn, null);
    }
    
    protected async Task<T> QueryAsync<T>(
        Func<NpgsqlConnection, Task<T>> query,
        CancellationToken ct)
    {
        if (_transactionScope != null)
        {
            var connection = (NpgsqlConnection)_transactionScope.Connection;
            return await query(connection);
        }

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        return await query(conn);
    }
}