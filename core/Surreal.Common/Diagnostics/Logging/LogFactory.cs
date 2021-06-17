using System;
using Surreal.Text;

namespace Surreal.Diagnostics.Logging {
  public interface ILogFactory {
    ILog GetLog(string category);
  }

  public static class LogFactory {
    public static ILogFactory Current { get; set; } = NullLogFactory.Instance;

    public static ILog GetLog<T>()             => GetLog(typeof(T));
    public static ILog GetLog(Type type)       => GetLog(type.GetFullNameWithoutGenerics());
    public static ILog GetLog(string category) => new LazyLog(category);

    private sealed class LazyLog : ILog {
      private readonly Lazy<ILog> log;

      public LazyLog(string category) {
        log = new Lazy<ILog>(() => Current.GetLog(category));
      }

      public bool IsLevelEnabled(LogLevel level)               => log.Value.IsLevelEnabled(level);
      public void WriteMessage(LogLevel level, string message) => log.Value.WriteMessage(level, message);
    }
  }
}