using System;
using System.Threading;

namespace Surreal.Diagnostics.Logging {
  public sealed class ConsoleLogFactory : ILogFactory {
    private readonly LogLevel     minLevel;
    private readonly LogFormatter formatter;

    public ConsoleLogFactory(LogLevel minLevel)
        : this(minLevel, LogFormatters.Default()) {
    }

    public ConsoleLogFactory(LogLevel minLevel, LogFormatter formatter) {
      this.minLevel  = minLevel;
      this.formatter = formatter;
    }

    public ILog GetLog(string category) {
      return new ConsoleLog(category, minLevel, formatter);
    }

    private sealed class ConsoleLog : ILog {
      private readonly string       category;
      private readonly LogLevel     minLevel;
      private readonly LogFormatter formatter;

      public ConsoleLog(string category, LogLevel minLevel, LogFormatter formatter) {
        this.category  = category;
        this.minLevel  = minLevel;
        this.formatter = formatter;
      }

      public bool IsLevelEnabled(LogLevel level)
        => level >= minLevel;

      public void WriteMessage(LogLevel level, string message)
        => Console.WriteLine(formatter(category, Thread.CurrentThread.ManagedThreadId, level, message));
    }
  }
}