using System.Data;
using Npgsql;
using Server.Application.Port;

namespace Server.Infrastructure.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly string _connectionString;
    private NpgsqlConnection? _connection;
    private NpgsqlTransaction? _transaction;
    private bool _disposed;
    
    public UnitOfWork(string connectionString)
    {
        _connectionString = connectionString ?? 
                            throw new ArgumentNullException(nameof(connectionString));
    }

    public IDbConnection Connection
    {
        get
        {
            ThrowIfDisposed();
            
            if (_connection?.State != ConnectionState.Open)
            {
                _connection?.Dispose();
                _connection = new NpgsqlConnection(_connectionString);
                _connection.Open();
            }
            return _connection;
        }
    }
    
    public IDbTransaction Transaction 
        => _transaction ?? throw new InvalidOperationException();
    
    public async Task BeginTransactionAsync()
    {
        ThrowIfDisposed();
        if (_transaction != null)
            throw new InvalidOperationException("TRANSACTION_ALREADY_STARTED");

        _transaction = await ((NpgsqlConnection)Connection).BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        ThrowIfDisposed();
        ThrowIfNoTransaction();
        
        await _transaction!.CommitAsync();
        
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackAsync()
    {
        ThrowIfDisposed();
        ThrowIfNoTransaction();

        await _transaction!.RollbackAsync();
        
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        _connection?.Dispose();
        _transaction?.Dispose();
        
        _disposed = true;
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
    
        if (_transaction != null)
            await _transaction.DisposeAsync();
        
        if (_connection != null)
            await _connection.DisposeAsync();
    
        _disposed = true;
    }
    
    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(UnitOfWork));
    }
    
    private void ThrowIfNoTransaction()
    {
        if (_transaction == null)
            throw new InvalidOperationException("No active transaction");
    }
}