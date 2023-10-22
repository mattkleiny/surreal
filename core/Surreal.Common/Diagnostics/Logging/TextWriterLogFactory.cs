namespace Surreal.Diagnostics.Logging;

/// <summary>
/// A <see cref="ILogFactory" /> that writes to the given <see cref="TextWriter" />.
/// </summary>
public sealed class TextWriterLogFactory(TextWriter writer, LogLevel minLevel, LogFormatter formatter) : ILogFactory
{
  private static readonly BlockingCollection<string> Messages = new();

  public TextWriterLogFactory(TextWriter writer, LogLevel minLevel)
    : this(writer, minLevel, LogFormatters.Default())
  {
    Task.Run(() => WriteLogsAsync(writer));
  }

  public ILog GetLog(string category)
  {
    return new TextWriterLog(writer, category, minLevel, formatter);
  }

  private static void WriteLogsAsync(TextWriter writer)
  {
    while (!Messages.IsCompleted)
    {
      var message = Messages.Take();

      writer.WriteLine(message);
    }
  }

  /// <summary>
  /// A <see cref="ILog" /> that writes to a <see cref="TextWriter" />.
  /// </summary>
  private sealed class TextWriterLog(TextWriter writer, string category, LogLevel minLevel, LogFormatter formatter) : ILog
  {
    public bool IsLevelEnabled(LogLevel level)
    {
      return level >= minLevel;
    }

    public void WriteMessage(LogLevel level, string message, Exception? exception = null)
    {
      Messages.Add(formatter(category, level, message, exception));
    }

    public void WriteMessage(LogLevel level, ref LogInterpolator handler, Exception? exception = null)
    {
      Messages.Add(formatter(category, level, handler.GetFormattedTextAndReturnToPool(), exception));
    }
  }
}
