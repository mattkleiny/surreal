namespace Surreal;

/// <summary>Helpers for working with <see cref="IDisposable" />.</summary>
public static class Disposables
{
  /// <summary>A no-op <see cref="IDisposable" /> implementation.</summary>
  public static IDisposable Null { get; } = Anonymous(() => { });

  /// <summary>Creates a new anonymous, delegate-based <see cref="IDisposable" /> implementation.</summary>
  public static IDisposable Anonymous(Action action)
  {
    return new AnonymousDisposable(action);
  }

  /// <summary>An anonymous, delegate-based <see cref="IDisposable" /> implementation.</summary>
  private sealed class AnonymousDisposable : IDisposable
  {
    private readonly Action _action;

    public AnonymousDisposable(Action action)
    {
      _action = action;
    }

    public void Dispose()
    {
      _action.Invoke();
    }
  }
}



