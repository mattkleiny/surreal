namespace Surreal.Diagnostics.Logging {
  public sealed class NullLogFactory : ILogFactory {
    public static readonly NullLogFactory Instance = new NullLogFactory();

    public ILog GetLog(string category) => NullLog.Instance;

    private sealed class NullLog : ILog {
// ReSharper disable once MemberHidesStaticFromOuterClass
      public static readonly NullLog Instance = new NullLog();

      public bool IsLevelEnabled(LogLevel level) => false;

      public void WriteMessage(LogLevel level, string message) {
        // no-op
      }
    }
  }
}