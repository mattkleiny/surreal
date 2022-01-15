namespace Surreal.Diagnostics.Logging;

/// <summary>A no-op <see cref="ILogFactory"/>.</summary>
public sealed class NullLogFactory : ILogFactory
{
  public static readonly NullLogFactory Instance = new();

  public ILog GetLog(string category) => new NullLog();

  /// <summary>A <see cref="ILog"/> that does nothing.</summary>
  private sealed class NullLog : ILog
  {
    public bool IsLevelEnabled(LogLevel level) => false;

    public void WriteMessage(LogLevel level, string message)
    {
      // no-op
    }

    public void WriteMessage(LogLevel level, ref LogInterpolator handler)
    {
      // no-op
    }
  }
}
