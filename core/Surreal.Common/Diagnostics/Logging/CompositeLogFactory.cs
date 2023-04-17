namespace Surreal.Diagnostics.Logging;

/// <summary>
/// A <see cref="ILogFactory" /> that composes multiple other <see cref="ILogFactory" />s.
/// </summary>
public sealed class CompositeLogFactory : ILogFactory
{
  private readonly ILogFactory[] _factories;

  public CompositeLogFactory(params ILogFactory[] factories)
  {
    _factories = factories;
  }

  public ILog GetLog(string category)
  {
    return new CompositeLog(_factories.Select(factory => factory.GetLog(category)));
  }

  /// <summary>
  /// A <see cref="ILog" /> that delegates to multiple other <see cref="ILog" />s.
  /// </summary>
  private sealed class CompositeLog : ILog
  {
    private readonly ILog[] _logs;

    public CompositeLog(IEnumerable<ILog> logs)
    {
      _logs = logs.ToArray();
    }

    public bool IsLevelEnabled(LogLevel level)
    {
      for (var i = 0; i < _logs.Length; i++)
      {
        var log = _logs[i];

        if (log.IsLevelEnabled(level))
        {
          return true;
        }
      }

      return false;
    }

    public void WriteMessage(LogLevel level, string message, Exception? exception = null)
    {
      for (var i = 0; i < _logs.Length; i++)
      {
        var log = _logs[i];
        if (log.IsLevelEnabled(level))
        {
          log.WriteMessage(level, message, exception);
        }
      }
    }

    public void WriteMessage(LogLevel level, ref LogInterpolator handler, Exception? exception = null)
    {
      string? cachedMessage = null;

      for (var i = 0; i < _logs.Length; i++)
      {
        var log = _logs[i];
        if (log.IsLevelEnabled(level))
        {
          cachedMessage ??= handler.GetFormattedTextAndReturnToPool();

          log.WriteMessage(level, cachedMessage, exception);
        }
      }
    }
  }
}
