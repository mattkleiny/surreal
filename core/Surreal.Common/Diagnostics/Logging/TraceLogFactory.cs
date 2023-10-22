namespace Surreal.Diagnostics.Logging;

/// <summary>
/// A <see cref="ILogFactory" /> that writes to the built-in .NET <see cref="Trace" /> console.
/// </summary>
public sealed class TraceLogFactory(LogLevel minLevel, LogFormatter formatter) : ILogFactory
{
  public ILog GetLog(string category)
  {
    return new TraceLog(category, minLevel, formatter);
  }

  /// <summary>
  /// A <see cref="ILog" /> that writes to <see cref="Trace" />.
  /// </summary>
  private sealed class TraceLog(string category, LogLevel minLevel, LogFormatter formatter) : ILog
  {
    public bool IsLevelEnabled(LogLevel level)
    {
      return level >= minLevel;
    }

    public void WriteMessage(LogLevel level, string message, Exception? exception = null)
    {
      Trace.WriteLine(formatter(category, level, message, exception));
    }

    public void WriteMessage(LogLevel level, ref LogInterpolator handler, Exception? exception = null)
    {
      Trace.WriteLine(formatter(category, level, handler.GetFormattedTextAndReturnToPool(), exception));
    }
  }
}
