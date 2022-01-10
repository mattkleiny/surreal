using Surreal.Text;

namespace Surreal.Diagnostics.Logging;

/// <summary>A <see cref="ILogFactory"/> that writes to the given <see cref="TextWriter"/>.</summary>
public sealed class TextWriterLogFactory : ILogFactory
{
  private readonly TextWriter   writer;
  private readonly LogLevel     minLevel;
  private readonly LogFormatter formatter;

  public TextWriterLogFactory(TextWriter writer, LogLevel minLevel)
    : this(writer, minLevel, LogFormatters.Default())
  {
  }

  public TextWriterLogFactory(TextWriter writer, LogLevel minLevel, LogFormatter formatter)
  {
    this.writer    = writer;
    this.minLevel  = minLevel;
    this.formatter = formatter;
  }

  public ILog GetLog(string category)
  {
    return new TextWriterLog(writer, category, minLevel, formatter);
  }

  /// <summary>A <see cref="ILog"/> that writes to a <see cref="TextWriter"/>.</summary>
  private sealed class TextWriterLog : ILog
  {
    private readonly TextWriter   writer;
    private readonly string       category;
    private readonly LogLevel     minLevel;
    private readonly LogFormatter formatter;

    public TextWriterLog(TextWriter writer, string category, LogLevel minLevel, LogFormatter formatter)
    {
      this.writer    = writer;
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
      writer.WriteLine(formatter(category, level, message));
    }

    public void WriteMessage(LogLevel level, ref LogInterpolator handler)
    {
      writer.WriteLine(formatter(category, level, handler.GetFormattedTextAndReturnToPool()));
    }
  }
}
