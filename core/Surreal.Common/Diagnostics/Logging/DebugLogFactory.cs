namespace Surreal.Diagnostics.Logging;

/// <summary>A <see cref="ILogFactory"/> that writes to the built-in .NET <see cref="Debug"/> console.</summary>
public sealed class DebugLogFactory : ILogFactory
{
  private readonly LogLevel minLevel;
  private readonly LogFormatter formatter;

  public DebugLogFactory(LogLevel minLevel)
    : this(minLevel, LogFormatters.Default())
  {
  }

  public DebugLogFactory(LogLevel minLevel, LogFormatter formatter)
  {
    this.minLevel  = minLevel;
    this.formatter = formatter;
  }

  public ILog GetLog(string category) => new DebugLog(category, minLevel, formatter);

  /// <summary>A <see cref="ILog"/> that writes to <see cref="Debug"/>.</summary>
  private sealed class DebugLog : ILog
  {
    private readonly string category;
    private readonly LogLevel minLevel;
    private readonly LogFormatter formatter;

    public DebugLog(string category, LogLevel minLevel, LogFormatter formatter)
    {
      this.category  = category;
      this.minLevel  = minLevel;
      this.formatter = formatter;
    }

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
