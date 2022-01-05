using System.Globalization;

namespace Surreal.Diagnostics.Logging;

/// <summary>Formats log messages with the given details.</summary>
public delegate string LogFormatter(string category, LogLevel level, string message);

/// <summary>Standard purpose <see cref="LogFormatter"/>s.</summary>
public static class LogFormatters
{
  /// <summary>A default <see cref="LogFormatter"/> with some basic details.</summary>
  public static LogFormatter Default(
    bool includeTime = true,
    bool includeThreadId = true,
    bool includeCategory = true,
    bool includeLevel = true)
  {
    return (category, level, message) =>
    {
      var builder = new StringBuilder();

      if (includeTime)
      {
        builder.Append(CultureInfo.InvariantCulture, $"{DateTime.Now:h:mm:ss tt} - ");
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
    };
  }
}
