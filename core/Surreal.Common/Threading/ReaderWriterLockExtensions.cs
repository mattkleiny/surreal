namespace Surreal.Threading;

/// <summary>Static extensions for reader/writer locks.</summary>
public static class ReaderWriterLockExtensions
{
  public static ReadLockScope  ScopeReadLock(this ReaderWriterLockSlim readerWriterLock)  => new(readerWriterLock);
  public static WriteLockScope ScopedWriteLock(this ReaderWriterLockSlim readerWriterLock) => new(readerWriterLock);

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
