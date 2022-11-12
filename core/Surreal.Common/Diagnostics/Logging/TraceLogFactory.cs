namespace Surreal.Diagnostics.Logging;

/// <summary>A <see cref="ILogFactory" /> that writes to the built-in .NET <see cref="Trace" /> console.</summary>
public sealed class TraceLogFactory : ILogFactory
{
  private readonly LogFormatter _formatter;
  private readonly LogLevel _minLevel;

  public TraceLogFactory(LogLevel minLevel)
    : this(minLevel, LogFormatters.Default())
  {
  }

  public TraceLogFactory(LogLevel minLevel, LogFormatter formatter)
  {
    _minLevel = minLevel;
    _formatter = formatter;
  }

  public ILog GetLog(string category)
  {
    return new TraceLog(category, _minLevel, _formatter);
  }

  /// <summary>A <see cref="ILog" /> that writes to <see cref="Trace" />.</summary>
  private sealed class TraceLog : ILog
  {
    private readonly string _category;
    private readonly LogFormatter _formatter;
    private readonly LogLevel _minLevel;

    public TraceLog(string category, LogLevel minLevel, LogFormatter formatter)
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
      Trace.WriteLine(_formatter(_category, level, message, exception));
    }

    public void WriteMessage(LogLevel level, ref LogInterpolator handler, Exception? exception = null)
    {
      Trace.WriteLine(_formatter(_category, level, handler.GetFormattedTextAndReturnToPool(), exception));
    }
  }
}

