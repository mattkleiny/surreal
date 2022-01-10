using Surreal.Text;

namespace Surreal.Diagnostics.Logging;

/// <summary>A <see cref="ILogFactory"/> that writes to the built-in .NET <see cref="Trace"/> console.</summary>
public sealed class TraceLogFactory : ILogFactory
{
  private readonly LogLevel     minLevel;
  private readonly LogFormatter formatter;

  public TraceLogFactory(LogLevel minLevel)
    : this(minLevel, LogFormatters.Default())
  {
  }

  public TraceLogFactory(LogLevel minLevel, LogFormatter formatter)
  {
    this.minLevel  = minLevel;
    this.formatter = formatter;
  }

  public ILog GetLog(string category) => new TraceLog(category, minLevel, formatter);

  /// <summary>A <see cref="ILog"/> that writes to <see cref="Trace"/>.</summary>
  private sealed class TraceLog : ILog
  {
    private readonly string       category;
    private readonly LogLevel     minLevel;
    private readonly LogFormatter formatter;

    public TraceLog(string category, LogLevel minLevel, LogFormatter formatter)
    {
      this.category  = category;
      this.minLevel  = minLevel;
      this.formatter = formatter;
    }

    public bool IsLevelEnabled(LogLevel level)
    {
      return level >= minLevel;
    }

    public void WriteMessage(LogLevel level, string message)
    {
      Trace.WriteLine(formatter(category, level, message));
    }

    public void WriteMessage(LogLevel level, ref LogInterpolator handler)
    {
      Trace.WriteLine(formatter(category, level, handler.GetFormattedTextAndReturnToPool()));
    }
  }
}
