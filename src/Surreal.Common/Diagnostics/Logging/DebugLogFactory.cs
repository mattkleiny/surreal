using System.Diagnostics;
using System.Threading;

namespace Surreal.Diagnostics.Logging
{
  public sealed class DebugLogFactory : ILogFactory
  {
    private readonly LogLevel     minLevel;
    private readonly LogFormatter formatter;

    public DebugLogFactory(LogLevel minLevel)
        : this(minLevel, LogFormatters.Default())
    {
    }

    public DebugLogFactory(LogLevel minLevel, LogFormatter formatter)
    {
      this.minLevel  = minLevel;
      this.formatter = formatter;
    }

    public ILog GetLog(string category) => new DebugLog(category, minLevel, formatter);

    private sealed class DebugLog : ILog
    {
      private readonly string       category;
      private readonly LogLevel     minLevel;
      private readonly LogFormatter formatter;

      public DebugLog(string category, LogLevel minLevel, LogFormatter formatter)
      {
        this.category  = category;
        this.minLevel  = minLevel;
        this.formatter = formatter;
      }

      public bool IsLevelEnabled(LogLevel level)
        => level >= minLevel;

      public void WriteMessage(LogLevel level, string message)
        => Debug.WriteLine(formatter(category, Thread.CurrentThread.ManagedThreadId, level, message));
    }
  }
}