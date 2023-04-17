namespace Surreal.Diagnostics.Logging;

/// <summary>
/// A <see cref="ILogFactory" /> that writes to the built-in .NET <see cref="Debug" /> console.
/// </summary>
public sealed class DebugLogFactory : ILogFactory
{
  private readonly LogFormatter _formatter;
  private readonly LogLevel _minLevel;

  public DebugLogFactory(LogLevel minLevel)
    : this(minLevel, LogFormatters.Default())
  {
  }

  public DebugLogFactory(LogLevel minLevel, LogFormatter formatter)
  {
    _minLevel = minLevel;
    _formatter = formatter;
  }

  public ILog GetLog(string category)
  {
    return new DebugLog(category, _minLevel, _formatter);
  }

  /// <summary>
  /// A <see cref="ILog" /> that writes to <see cref="Debug" />.
  /// </summary>
  private sealed class DebugLog : ILog
  {
    private readonly string _category;
    private readonly LogFormatter _formatter;
    private readonly LogLevel _minLevel;

    public DebugLog(string category, LogLevel minLevel, LogFormatter formatter)
    {
      _category = category;
      _minLevel = minLevel;
      _formatter = formatter;
    }

    public bool IsLevelEnabled(LogLevel level)
    {
      return level >= _minLevel;
    }

    public void WriteMessage(LogLevel level, string message, Exception? exception = null)
    {
      Debug.WriteLine(_formatter(_category, level, message, exception));
    }

    public void WriteMessage(LogLevel level, ref LogInterpolator handler, Exception? exception = null)
    {
      Debug.WriteLine(_formatter(_category, level, handler.GetFormattedTextAndReturnToPool(), exception));
    }
  }
}
