﻿using System.Diagnostics.CodeAnalysis;
using Surreal.Text;

namespace Surreal.Diagnostics.Logging;

/// <summary>A factory for <see cref="ILog"/>s.</summary>
public interface ILogFactory
{
  ILog GetLog(string category);
}

/// <summary>Entry point for <see cref="ILogFactory"/>s.</summary>
public static class LogFactory
{
  public static ILogFactory Current { get; set; } = NullLogFactory.Instance;

  public static ILog GetLog<T>() => GetLog(typeof(T));
  public static ILog GetLog(Type type) => GetLog(type.GetFullNameWithoutGenerics());
  public static ILog GetLog(string category) => new LazyLog(category);

  /// <summary>A <see cref="ILog"/> that lazily acquires the <see cref="ILog"/> target.</summary>
  private sealed class LazyLog : ILog
  {
    private readonly Lazy<ILog> log;

    public LazyLog(string category)
    {
      log = new Lazy<ILog>(() => Current.GetLog(category));
    }

    public bool IsLevelEnabled(LogLevel level)
    {
      return log.Value.IsLevelEnabled(level);
    }

    public void WriteMessage(LogLevel level, string message)
    {
      log.Value.WriteMessage(level, message);
    }

    public void WriteMessage(LogLevel level, ref LogInterpolator handler)
    {
      log.Value.WriteMessage(level, ref handler);
    }
  }

  /// <summary>A no-op <see cref="ILogFactory"/>.</summary>
  [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
  private sealed class NullLogFactory : ILogFactory
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
}
