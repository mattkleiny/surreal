using System.Text;

namespace Surreal.Diagnostics.Logging;

public delegate string LogFormatter(string category, LogLevel level, string message);

public static class LogFormatters
{
  public static LogFormatter Default(
    bool includeTime = true,
    bool includeThreadId = true,
    bool includeCategory = true,
    bool includeLevel = true
  )
  {
    // re-use the string builder over each invocation; saves some bytes.
    var builder = new StringBuilder();

    return (category, level, message) =>
    {
      lock (builder)
      {
        builder.Clear();

        if (includeTime)
        {
          builder.Append($"{DateTime.Now:h:mm:ss tt} - ");
        }

        if (includeThreadId)
        {
          builder.Append("<thread ").Append(Environment.CurrentManagedThreadId).Append("> ");
        }

        if (includeCategory)
        {
          builder.Append(category);
        }

        if (includeLevel)
        {
          builder.Append(level switch
          {
            LogLevel.Trace => " [TRACE]: ",
            LogLevel.Debug => " [DEBUG]: ",
            LogLevel.Warn  => " [WARN]: ",
            LogLevel.Error => " [ERROR]: ",
            LogLevel.Fatal => " [FATAL]: ",

            _ => throw new ArgumentOutOfRangeException(nameof(level), level, $"An unrecognized log level was supplied: {level}"),
          });
        }

        builder.Append(message);

        return builder.ToString();
      }
    };
  }
}
