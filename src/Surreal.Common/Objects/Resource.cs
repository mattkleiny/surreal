using System;

namespace Surreal.Objects
{
  /// <summary>An application resource that can be deterministically destroyed.</summary>
  public abstract class Resource : IDisposable
  {
    public bool IsDisposed { get; private set; }

    public void Dispose()
    {
      if (!IsDisposed)
      {
        Dispose(true);
        IsDisposed = true;
      }
    }

    protected virtual void Dispose(bool managed)
    {
    }
  }
}
