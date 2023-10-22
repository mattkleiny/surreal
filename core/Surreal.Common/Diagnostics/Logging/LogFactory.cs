namespace Surreal.Diagnostics.Logging;

/// <summary>
/// A factory for <see cref="ILog" />s.
/// </summary>
public interface ILogFactory
{
  ILog GetLog(string category);
}

/// <summary>
/// Entry point for <see cref="ILogFactory" />s.
/// </summary>
public static class LogFactory
{
  public static ILogFactory Current { get; set; } = NullLogFactory.Instance;

  public static ILog GetLog<T>()
  {
    return GetLog(typeof(T));
  }

  public static ILog GetLog(Type type)
  {
    return GetLog(GetFullNameWithoutGenerics(type));
  }

  public static ILog GetLog(string category)
  {
    return Current.GetLog(category);
  }

  private static string GetFullNameWithoutGenerics(Type type)
  {
    static string RemoveGenerics(string value)
    {
      var index = value.IndexOf('`');

      return index == -1 ? value : value[..index];
    }

    return RemoveGenerics(type.FullName ?? string.Empty);
  }

  /// <summary>
  /// A no-op <see cref="ILogFactory" />.
  /// </summary>
  [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
  private sealed class NullLogFactory : ILogFactory
  {
    public static readonly NullLogFactory Instance = new();

    public ILog GetLog(string category)
    {
      return new NullLog();
    }

    /// <summary>
    /// A <see cref="ILog" /> that does nothing.
    /// </summary>
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
