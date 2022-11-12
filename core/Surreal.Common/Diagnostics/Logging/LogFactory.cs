using Surreal.Text;

namespace Surreal.Diagnostics.Logging;

/// <summary>A factory for <see cref="ILog" />s.</summary>
public interface ILogFactory
{
  ILog GetLog(string category);
}

/// <summary>Entry point for <see cref="ILogFactory" />s.</summary>
public static class LogFactory
{
  public static ILogFactory Current { get; set; } = NullLogFactory.Instance;

  public static ILog GetLog<T>()
  {
    return GetLog(typeof(T));
  }

  public static ILog GetLog(Type type)
  {
    return GetLog(type.GetFullNameWithoutGenerics());
  }

  public static ILog GetLog(string category)
  {
    return new LazyLog(category);
  }

  /// <summary>A <see cref="ILog" /> that lazily acquires the <see cref="ILog" /> target.</summary>
  private sealed class LazyLog : ILog
  {
    private readonly Lazy<ILog> _log;

    public LazyLog(string category)
    {
      _log = new Lazy<ILog>(() => Current.GetLog(category));
    }

    public bool IsLevelEnabled(LogLevel level)
    {
      return _log.Value.IsLevelEnabled(level);
    }

    public void WriteMessage(LogLevel level, string message, Exception? exception = null)
    {
      _log.Value.WriteMessage(level, message, exception);
    }

    public void WriteMessage(LogLevel level, ref LogInterpolator handler, Exception? exception = null)
    {
      _log.Value.WriteMessage(level, ref handler, exception);
    }
  }

  /// <summary>A no-op <see cref="ILogFactory" />.</summary>
  [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
  private sealed class NullLogFactory : ILogFactory
  {
    public static readonly NullLogFactory Instance = new();

    public ILog GetLog(string category)
    {
      return new NullLog();
    }

    /// <summary>A <see cref="ILog" /> that does nothing.</summary>
    private sealed class NullLog : ILog
    {
      public bool IsLevelEnabled(LogLevel level)
      {
        return false;
      }

      public void WriteMessage(LogLevel level, string message, Exception? exception = null)
      {
        // no-op
      }

      public void WriteMessage(LogLevel level, ref LogInterpolator handler, Exception? exception = null)
      {
        // no-op
      }
    }
  }
}


