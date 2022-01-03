namespace Surreal.Objects;

#pragma warning disable S3881

/// <summary>An application resource that can be deterministically destroyed.</summary>
public abstract class Resource : IDisposable
{
  ~Resource()
  {
    Dispose(false);
  }

  public bool IsDisposed { get; private set; }

  public void Dispose()
  {
    if (!IsDisposed)
    {
      Dispose(true);
      GC.SuppressFinalize(this);

      IsDisposed = true;
    }
  }

  protected virtual void Dispose(bool managed)
  {
  }
}
