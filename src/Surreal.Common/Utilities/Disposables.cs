using System;

namespace Surreal.Utilities
{
  public static class Disposables
  {
    public static AnonymousDisposable Anonymous(Action onDispose) => new(onDispose);

    public readonly struct AnonymousDisposable : IDisposable
    {
      private readonly Action onDispose;

      public AnonymousDisposable(Action onDispose)
      {
        this.onDispose = onDispose;
      }

      public void Dispose()
      {
        onDispose.Invoke();
      }
    }
  }
}