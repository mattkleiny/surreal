namespace Surreal.Diagnostics.Logging;

/// <summary>
/// A <see cref="ILogFactory" /> that writes to the given <see cref="TextWriter" />.
/// </summary>
public sealed class TextWriterLogFactory(TextWriter writer, LogLevel minLevel, LogFormatter formatter) : ILogFactory
{
  public ILog GetLog(string category)
  {
    return new TextWriterLog(writer, category, minLevel, formatter);
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
      writer.WriteLine(formatter(category, level, message, exception));
    }

    public void WriteMessage(LogLevel level, ref LogInterpolator handler, Exception? exception = null)
    {
      writer.WriteLine(formatter(category, level, handler.GetFormattedTextAndReturnToPool(), exception));
    }
  }
}
