namespace Surreal.Diagnostics.Logging;

/// <summary>
/// A <see cref="ILogFactory" /> that writes to the given <see cref="TextWriter" />.
/// </summary>
public sealed class TextWriterLogFactory(TextWriter writer, LogLevel minLevel, LogFormatter formatter) : ILogFactory
{
  private readonly LogFormatter _formatter = formatter;
  private readonly LogLevel _minLevel = minLevel;
  private readonly TextWriter _writer = writer;

  public TextWriterLogFactory(TextWriter writer, LogLevel minLevel)
    : this(writer, minLevel, LogFormatters.Default())
  {
  }

  public ILog GetLog(string category)
  {
    return new TextWriterLog(_writer, category, _minLevel, _formatter);
  }

  /// <summary>
  /// A <see cref="ILog" /> that writes to a <see cref="TextWriter" />.
  /// </summary>
  private sealed class TextWriterLog
    (TextWriter writer, string category, LogLevel minLevel, LogFormatter formatter) : ILog
  {
    private readonly string _category = category;
    private readonly LogFormatter _formatter = formatter;
    private readonly LogLevel _minLevel = minLevel;
    private readonly TextWriter _writer = writer;

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
