using Surreal.Text;

namespace Surreal.Diagnostics.Logging;

/// <summary>A <see cref="ILogFactory"/> that composes multiple other <see cref="ILogFactory"/>s.</summary>
public sealed class CompositeLogFactory : ILogFactory
{
  private readonly ILogFactory[] factories;

  public CompositeLogFactory(params ILogFactory[] factories)
  {
    this.factories = factories;
  }

  public ILog GetLog(string category)
  {
    return new CompositeLog(factories.Select(factory => factory.GetLog(category)));
  }

  /// <summary>A <see cref="ILog"/> that delegates to multiple other <see cref="ILog"/>s.</summary>
  private sealed class CompositeLog : ILog
  {
    private readonly ILog[] logs;

    public CompositeLog(IEnumerable<ILog> logs)
    {
      this.logs = logs.ToArray();
    }

    public bool IsLevelEnabled(LogLevel level)
    {
      for (var i = 0; i < logs.Length; i++)
      {
        var log = logs[i];

        if (log.IsLevelEnabled(level)) return true;
      }

      return false;
    }

    public void WriteMessage(LogLevel level, string message)
    {
      for (var i = 0; i < logs.Length; i++)
      {
        var log = logs[i];
        if (log.IsLevelEnabled(level))
        {
          log.WriteMessage(level, message);
        }
      }
    }

    public void WriteMessage(LogLevel level, ref PooledInterpolatedString handler)
    {
      for (var i = 0; i < logs.Length; i++)
      {
        var log = logs[i];
        if (log.IsLevelEnabled(level))
        {
          log.WriteMessage(level, ref handler);
        }
      }
    }
  }
}
