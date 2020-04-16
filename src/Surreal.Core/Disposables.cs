using System;
using System.Collections.Generic;

namespace Surreal
{
  public static class Disposables
  {
    public static IDisposable Anonymous(Action action)
    {
      return new AnonymousDisposable(action);
    }

    public static IDisposable Aggregate(IReadOnlyList<IDisposable> disposables)
    {
      return Anonymous(() =>
      {
        for (var i = 0; i < disposables.Count; i++)
        {
          disposables[i].Dispose();
        }
      });
    }

    private sealed class AnonymousDisposable : IDisposable
    {
      private readonly Action action;

      public AnonymousDisposable(Action action)
      {
        this.action = action;
      }

      public void Dispose()
      {
        action.Invoke();
      }
    }
  }
}
