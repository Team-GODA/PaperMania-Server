using System.Data;
using Npgsql;
using Server.Application.Port.Output.Infrastructure;

namespace Server.Infrastructure.Dao;

public class TransactionScope : ITransactionScope
{
    private readonly string _connectionString;
    private readonly ILogger<TransactionScope>? _logger;
    private NpgsqlConnection? _connection;
    private NpgsqlTransaction? _transaction;
    private bool _disposed;
    
    public TransactionScope(string connectionString, ILogger<TransactionScope>? logger = null)
    {
        _connectionString = connectionString ?? 
                            throw new ArgumentNullException(nameof(connectionString));
        _logger = logger;
    }

    public IDbConnection Connection
    {
        get
        {
            ThrowIfDisposed();
            EnsureConnection();
            return _connection!;
        }
    }
    
    public IDbTransaction Transaction 
    {
        get
        {
            ThrowIfDisposed();
            ThrowIfNoTransaction();
            return _transaction!;
        }
    }
    
    private async Task EnsureConnectionAsync()
    {
        if (_connection?.State == ConnectionState.Open)
            return;

        if (_transaction != null)
            throw new InvalidOperationException("TRANSACTION_ALREADY_STARTED");

        try
        {
            if (_connection != null)
                await _connection.DisposeAsync();
            
            _connection = new NpgsqlConnection(_connectionString);
            await _connection.OpenAsync();
            
            _logger?.LogDebug("데이터베이스 연결 열림");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "데이터베이스 연결 실패");
            throw;
        }
    }
    
    private void EnsureConnection()
    {
        if (_connection?.State == ConnectionState.Open)
            return;

        if (_transaction != null)
            throw new InvalidOperationException(
                "TRANSACTION_ALREADY_STARTED");

        try
        {
            _connection?.Dispose();
            _connection = new NpgsqlConnection(_connectionString);
            _connection.Open();
            
            _logger?.LogDebug("데이터베이스 연결 열림");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "데이터베이스 연결 실패");
            throw;
        }
    }
    
    public async Task BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        ThrowIfDisposed();
        
        if (_transaction != null)
            throw new InvalidOperationException("TRANSACTION_ALREADY_STARTED");

        try
        {
            await EnsureConnectionAsync();
            _transaction = await _connection!.BeginTransactionAsync(isolationLevel);
            
            _logger?.LogDebug("트랜잭션 시작: IsolationLevel = {IsolationLevel}", isolationLevel);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "트랜잭션 시작 실패");
            throw;
        }
    }

    public async Task CommitAsync()
    {
        ThrowIfDisposed();
        ThrowIfNoTransaction();
        
        try
        {
            await _transaction!.CommitAsync();
            _logger?.LogDebug("트랜잭션 커밋 성공");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "트랜잭션 커밋 실패");
            throw;
        }
        finally
        {
            await CleanupTransactionAsync();
        }
    }

    public async Task RollbackAsync()
    {
        ThrowIfDisposed();
        ThrowIfNoTransaction();

        try
        {
            await _transaction!.RollbackAsync();
            _logger?.LogDebug("트랜잭션 롤백 완료");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "트랜잭션 롤백 실패");
            throw;
        }
        finally
        {
            await CleanupTransactionAsync();
        }
    }

    private async Task CleanupTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        Dispose(false);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            if (_transaction != null)
            {
                try
                {
                    _transaction.Rollback();
                    _logger?.LogWarning("Dispose 시 활성 트랜잭션 자동 롤백");
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Dispose 중 트랜잭션 롤백 실패");
                }
                finally
                {
                    _transaction.Dispose();
                    _transaction = null;
                }
            }

            _connection?.Dispose();
            _connection = null;
        }

        _disposed = true;
    }

    private async ValueTask DisposeAsyncCore()
    {
        if (_disposed) return;

        if (_transaction != null)
        {
            try
            {
                await _transaction.RollbackAsync();
                _logger?.LogWarning("DisposeAsync 시 활성 트랜잭션 자동 롤백");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "DisposeAsync 중 트랜잭션 롤백 실패");
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        if (_connection != null)
        {
            await _connection.DisposeAsync();
            _connection = null;
        }

        _disposed = true;
    }
    
    public async Task ExecuteAsync(Func<Task> action)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        await BeginTransactionAsync();
        
        try
        {
            await action();
            await CommitAsync();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "트랜잭션 실행 중 오류 발생, 롤백 시도");
            
            try
            {
                await RollbackAsync();
            }
            catch (Exception rollbackEx)
            {
                _logger?.LogError(rollbackEx, "롤백 실패");
            }
            
            throw;
        }
    }
    
    public async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        await BeginTransactionAsync();
        
        try
        {
            var result = await action();
            await CommitAsync();
            return result;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "트랜잭션 실행 중 오류 발생, 롤백 시도");
            
            try
            {
                await RollbackAsync();
            }
            catch (Exception rollbackEx)
            {
                _logger?.LogError(rollbackEx, "롤백 실패");
            }
            
            throw;
        }
    }
    
    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(TransactionScope));
    }
    
    private void ThrowIfNoTransaction()
    {
        if (_transaction == null)
            throw new InvalidOperationException(
                "NOT_ACTIVE_TRANSACTION");
    }
}