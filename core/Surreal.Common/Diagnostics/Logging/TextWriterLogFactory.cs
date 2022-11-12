namespace Surreal.Diagnostics.Logging;

/// <summary>A <see cref="ILogFactory" /> that writes to the given <see cref="TextWriter" />.</summary>
public sealed class TextWriterLogFactory : ILogFactory
{
  private readonly LogFormatter _formatter;
  private readonly LogLevel _minLevel;
  private readonly TextWriter _writer;

  public TextWriterLogFactory(TextWriter writer, LogLevel minLevel)
    : this(writer, minLevel, LogFormatters.Default())
  {
  }

  public TextWriterLogFactory(TextWriter writer, LogLevel minLevel, LogFormatter formatter)
  {
    _writer = writer;
    _minLevel = minLevel;
    _formatter = formatter;
  }

  public ILog GetLog(string category)
  {
    return new TextWriterLog(_writer, category, _minLevel, _formatter);
  }

  /// <summary>A <see cref="ILog" /> that writes to a <see cref="TextWriter" />.</summary>
  private sealed class TextWriterLog : ILog
  {
    private readonly string _category;
    private readonly LogFormatter _formatter;
    private readonly LogLevel _minLevel;
    private readonly TextWriter _writer;

    public TextWriterLog(TextWriter writer, string category, LogLevel minLevel, LogFormatter formatter)
    {
      _writer = writer;
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
      _writer.WriteLine(_formatter(_category, level, message, exception));
    }

    public void WriteMessage(LogLevel level, ref LogInterpolator handler, Exception? exception = null)
    {
      _writer.WriteLine(_formatter(_category, level, handler.GetFormattedTextAndReturnToPool(), exception));
    }
  }
}

