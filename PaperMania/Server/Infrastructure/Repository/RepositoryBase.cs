using Npgsql;
using Server.Application.Port;

namespace Server.Infrastructure.Repository;

public class RepositoryBase
{ 
    private readonly string _connectionString;
    private readonly IUnitOfWork? _unitOfWork;

    protected RepositoryBase(
        string connectionString,
        IUnitOfWork? unitOfWork)
    {
        _connectionString = connectionString;
        _unitOfWork = unitOfWork;
    }

    protected async Task<T> ExecuteAsync<T>(
        Func<NpgsqlConnection, NpgsqlTransaction?, Task<T>> query)
    {
        if (_unitOfWork != null && _unitOfWork.Transaction != null)
        {
            var connection = (NpgsqlConnection)_unitOfWork.Connection;
            var transaction = _unitOfWork.Transaction as NpgsqlTransaction;
            return await query(connection, transaction);
        }

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        return await query(conn, null);
    }
    
    protected async Task ExecuteAsync(
        Func<NpgsqlConnection, NpgsqlTransaction?, Task> query)
    {
        if (_unitOfWork != null && _unitOfWork.Transaction != null)
        {
            var connection = (NpgsqlConnection)_unitOfWork.Connection;
            var transaction = _unitOfWork.Transaction as NpgsqlTransaction;
            await query(connection, transaction);
            return;
        }

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        await query(conn, null);
    }
    
    protected async Task<T> QueryAsync<T>(
        Func<NpgsqlConnection, Task<T>> query)
    {
        if (_unitOfWork?.Transaction != null)
        {
            var connection = (NpgsqlConnection)_unitOfWork.Connection;
            return await query(connection);
        }

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        return await query(conn);
    }
}