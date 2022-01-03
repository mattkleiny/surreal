namespace Surreal.Diagnostics.Logging;

public sealed class NullLogFactory : ILogFactory
{
  public static readonly NullLogFactory Instance = new();

  public ILog GetLog(string category) => new NullLog();

  private sealed class NullLog : ILog
  {
    public bool IsLevelEnabled(LogLevel level) => false;

    public void WriteMessage(LogLevel level, string message)
    {
      // no-op
    }
  }
}
