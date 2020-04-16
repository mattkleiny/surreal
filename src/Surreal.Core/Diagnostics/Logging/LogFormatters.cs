using System;
using System.Text;

namespace Surreal.Diagnostics.Logging
{
  public delegate string LogFormatter(string category, int threadId, LogLevel level, string message);

  public static class LogFormatters
  {
    public static LogFormatter Default(bool includeTime = true, bool includeThreadId = true, bool includeCategory = true, bool includeLevel = true)
    {
      // re-use the string builder over each invocation; saves some bytes.
      var builder = new StringBuilder();

      return (category, threadId, level, message) =>
      {
        lock (builder)
        {
          builder.Clear();

          if (includeTime) builder.AppendFormat("{0:h:mm:ss tt} - ", DateTime.Now);
          if (includeThreadId) builder.Append("<thread ").Append(threadId).Append("> ");
          if (includeCategory) builder.Append(category);

          if (includeLevel)
          {
            switch (level)
            {
              case LogLevel.Trace:
                builder.Append(" [TRACE]: ");
                break;
              case LogLevel.Debug:
                builder.Append(" [DEBUG]: ");
                break;
              case LogLevel.Warn:
                builder.Append(" [WARN]: ");
                break;
              case LogLevel.Error:
                builder.Append(" [ERROR]: ");
                break;
              case LogLevel.Fatal:
                builder.Append(" [FATAL]: ");
                break;

              default:
                throw new ArgumentException("An unrecognized log level was supplied: " + level);
            }
          }

          builder.Append(message);

          return builder.ToString();
        }
      };
    }
  }
}
