namespace Surreal.Threading;

/// <summary>Static extensions for reader/writer locks.</summary>
public static class LockExtensions
{
  public static ReadLockScope ScopeReadLock(this ReaderWriterLockSlim readerWriterLock)
  {
    return new ReadLockScope(readerWriterLock);
  }

  public static WriteLockScope ScopedWriteLock(this ReaderWriterLockSlim readerWriterLock)
  {
    return new WriteLockScope(readerWriterLock);
  }

  /// <summary>Scopes a read lock to the given <see cref="IDisposable"/>.</summary>
  public readonly struct ReadLockScope : IDisposable
  {
    private readonly ReaderWriterLockSlim readerWriterLock;

    public ReadLockScope(ReaderWriterLockSlim readerWriterLock)
    {
      this.readerWriterLock = readerWriterLock;

      readerWriterLock.EnterReadLock();
    }

    public void Dispose()
    {
      readerWriterLock.ExitReadLock();
    }
  }

  /// <summary>Scopes a write lock to the given <see cref="IDisposable"/>.</summary>
  public readonly struct WriteLockScope : IDisposable
  {
    private readonly ReaderWriterLockSlim readerWriterLock;

    public WriteLockScope(ReaderWriterLockSlim readerWriterLock)
    {
      this.readerWriterLock = readerWriterLock;

      readerWriterLock.EnterWriteLock();
    }

    public void Dispose()
    {
      readerWriterLock.ExitWriteLock();
    }
  }
}
