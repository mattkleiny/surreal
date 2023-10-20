namespace Surreal.Diagnostics.Logging;

/// <summary>
/// A <see cref="ILogFactory" /> that writes to the built-in .NET <see cref="Debug" /> console.
/// </summary>
public sealed class DebugLogFactory(LogLevel minLevel, LogFormatter formatter) : ILogFactory
{
  public DebugLogFactory(LogLevel minLevel)
    : this(minLevel, LogFormatters.Default())
  {
  }

  public ILog GetLog(string category)
  {
    return new DebugLog(category, minLevel, formatter);
  }

  /// <summary>
  /// A <see cref="ILog" /> that writes to <see cref="Debug" />.
  /// </summary>
  private sealed class DebugLog(string category, LogLevel minLevel, LogFormatter formatter) : ILog
  {
    public bool IsLevelEnabled(LogLevel level)
    {
      return level >= minLevel;
    }

    public void WriteMessage(LogLevel level, string message, Exception? exception = null)
    {
      Debug.WriteLine(formatter(category, level, message, exception));
    }

    public void WriteMessage(LogLevel level, ref LogInterpolator handler, Exception? exception = null)
    {
      Debug.WriteLine(formatter(category, level, handler.GetFormattedTextAndReturnToPool(), exception));
    }
  }
}
