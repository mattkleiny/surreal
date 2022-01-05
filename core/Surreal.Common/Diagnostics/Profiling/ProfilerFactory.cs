using Surreal.Text;

namespace Surreal.Diagnostics.Profiling;

/// <summary>A factory for <see cref="IProfiler"/>s.</summary>
public interface IProfilerFactory
{
  IProfiler GetProfiler(string category);
}

/// <summary>Entry point for <see cref="IProfilerFactory"/>s.</summary>
public static class ProfilerFactory
{
  public static IProfilerFactory Current { get; set; } = NullProfilerFactory.Instance;

  public static IProfiler GetProfiler<T>()             => GetProfiler(typeof(T));
  public static IProfiler GetProfiler(Type type)       => GetProfiler(type.GetFullNameWithoutGenerics());
  public static IProfiler GetProfiler(string category) => new LazyProfiler(category);

  /// <summary>A <see cref="IProfiler"/> that lazily acquires the <see cref="IProfiler"/> target.</summary>
  private sealed class LazyProfiler : IProfiler
  {
    private readonly Lazy<IProfiler> profiler;

    public LazyProfiler(string category)
    {
      profiler = new Lazy<IProfiler>(() => Current.GetProfiler(category));
    }

    public ProfilingScope Track(string task)
    {
      return profiler.Value.Track(task);
    }

    public ProfilingScope Track(string category, string task)
    {
      return profiler.Value.Track(category, task);
    }
  }
}
