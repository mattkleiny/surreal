using Surreal.Text;

namespace Surreal.Diagnostics.Profiling;

/// <summary>
/// A factory for <see cref="IProfiler" />s.
/// </summary>
public interface IProfilerFactory
{
  IProfiler GetProfiler(string category);
}

/// <summary>
/// Entry point for <see cref="IProfilerFactory" />s.
/// </summary>
public static class ProfilerFactory
{
  public static IProfilerFactory Current { get; set; } = NullProfilerFactory.Instance;

  public static IProfiler GetProfiler<T>()
  {
    return GetProfiler(typeof(T));
  }

  public static IProfiler GetProfiler(Type type)
  {
    return GetProfiler(type.GetFullNameWithoutGenerics());
  }

  public static IProfiler GetProfiler(string category)
  {
    return new LazyProfiler(category);
  }

  /// <summary>
  /// A <see cref="IProfiler" /> that lazily acquires the <see cref="IProfiler" /> target.
  /// </summary>
  private sealed class LazyProfiler : IProfiler
  {
    private readonly Lazy<IProfiler> _profiler;

    public LazyProfiler(string category)
    {
      _profiler = new Lazy<IProfiler>(() => Current.GetProfiler(category));
    }

    public ProfilingScope Track(string task)
    {
      return _profiler.Value.Track(task);
    }

    public ProfilingScope Track(string category, string task)
    {
      return _profiler.Value.Track(category, task);
    }
  }

  /// <summary>
  /// A <see cref="IProfilerFactory" /> that does nothing.
  /// </summary>
  [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
  private sealed class NullProfilerFactory : IProfilerFactory
  {
    public static readonly NullProfilerFactory Instance = new();

    public IProfiler GetProfiler(string category)
    {
      return new NullProfiler();
    }

    /// <summary>
    /// A no-op <see cref="IProfiler" />.
    /// </summary>
    private sealed class NullProfiler : IProfiler
    {
      public ProfilingScope Track(string task)
      {
        return default;
      }

      public ProfilingScope Track(string category, string task)
      {
        return default;
      }
    }
  }
}
