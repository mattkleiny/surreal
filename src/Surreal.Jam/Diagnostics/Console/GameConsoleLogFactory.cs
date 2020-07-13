using System.Threading;
using Surreal.Diagnostics.Logging;

namespace Surreal.Diagnostics.Console {
  public sealed class GameConsoleLogFactory : ILogFactory {
    private readonly IGameConsole console;
    private readonly LogLevel     minLevel;
    private readonly LogFormatter formatter;

    public GameConsoleLogFactory(IGameConsole console, LogLevel minLevel)
        : this(console, minLevel, LogFormatters.Default()) {
    }

    public GameConsoleLogFactory(IGameConsole console, LogLevel minLevel, LogFormatter formatter) {
      this.console   = console;
      this.minLevel  = minLevel;
      this.formatter = formatter;
    }

    public ILog GetLog(string category) => new GameConsoleLog(console, category, minLevel, formatter);

    private sealed class GameConsoleLog : ILog {
      private readonly IGameConsole console;
      private readonly string       category;
      private readonly LogLevel     minLevel;
      private readonly LogFormatter formatter;

      public GameConsoleLog(IGameConsole console, string category, LogLevel minLevel, LogFormatter formatter) {
        this.console   = console;
        this.category  = category;
        this.minLevel  = minLevel;
        this.formatter = formatter;
      }

      public bool IsLevelEnabled(LogLevel level)
        => level >= minLevel;

      public void WriteMessage(LogLevel level, string message)
        => console.WriteLine(formatter(category, Thread.CurrentThread.ManagedThreadId, level, message));
    }
  }
}