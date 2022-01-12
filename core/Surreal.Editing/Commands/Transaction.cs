namespace Surreal.Commands;

/// <summary>An operation that can be added to a <see cref="Transaction"/>.</summary>
public interface ITransactionOperation
{
  ValueTask CommitAsync(CancellationToken cancellationToken = default);
  ValueTask RollbackAsync(CancellationToken cancellationToken = default);
}

/// <summary>A transaction that can occur across the application.</summary>
public sealed class Transaction : IDisposable
{
  private static readonly AsyncLocal<Transaction?> AsyncLocalTransaction = new();

  public static Transaction Current
  {
    get
    {
      var transaction = AsyncLocalTransaction.Value;

      if (transaction == null)
      {
        throw new TransactionException("There is currently no on-going transaction");
      }

      return transaction;
    }
  }

  /// <summary>Creates a new <see cref="Transaction"/> in this async context.</summary>
  public static Transaction Create()
  {
    return AsyncLocalTransaction.Value = new Transaction();
  }

  private readonly List<ITransactionOperation> operations = new();
  private          TransactionStatus           status;

  public void Push(ITransactionOperation operation)
  {
    if (status != TransactionStatus.Uncommitted)
    {
      throw new TransactionException("The transaction has already been processed, cannot add new operations");
    }

    lock (operations)
    {
      operations.Add(operation);
    }
  }

  public void Cancel()
  {
    if (status != TransactionStatus.Uncommitted)
    {
      throw new TransactionException("The transaction has already been processed, cannot cancel operations");
    }

    lock (operations)
    {
      operations.Clear();
    }

    if (Current == this)
    {
      AsyncLocalTransaction.Value = null;
    }

    status = TransactionStatus.Cancelled;
  }

  public async ValueTask CommitAsync(CancellationToken cancellationToken = default)
  {
    if (status != TransactionStatus.Uncommitted)
    {
      throw new TransactionException("The transaction has already been processed, cannot commit");
    }

    Monitor.Enter(operations);

    try
    {
      for (var i = 0; i < operations.Count; i++)
      {
        var operation = operations[i];

        await operation.CommitAsync(cancellationToken);
      }
    }
    catch (Exception)
    {
      throw new NotImplementedException(); // TODO: handle exceptions
    }
    finally
    {
      Monitor.Exit(operations);
    }

    status = TransactionStatus.RolledForward;
  }

  public async ValueTask RollbackAsync(CancellationToken cancellationToken = default)
  {
    if (status != TransactionStatus.RolledForward)
    {
      throw new TransactionException("The transaction has already been committed, cannot rollback");
    }

    Monitor.Enter(operations);

    try
    {
      for (var i = operations.Count - 1; i >= 0; i--)
      {
        var operation = operations[i];

        await operation.RollbackAsync(cancellationToken);
      }
    }
    catch (Exception)
    {
      throw new NotImplementedException(); // TODO: handle exceptions
    }
    finally
    {
      Monitor.Exit(operations);
    }

    status = TransactionStatus.RolledBack;
  }

  public void Dispose()
  {
    if (status == TransactionStatus.Uncommitted)
    {
      Cancel();
    }
  }

  private enum TransactionStatus
  {
    Uncommitted,
    RolledForward,
    RolledBack,
    Cancelled,
  }
}

/// <summary>An exception when dealing with <see cref="Transaction"/>s.</summary>
public class TransactionException : Exception
{
  public TransactionException(string message, Exception? innerException = null)
    : base(message, innerException)
  {
  }
}
