namespace Surreal.Diagnostics.Profiling;

/// <summary>A <see cref="IProfilerFactory"/> that does nothing.</summary>
public sealed class NullProfilerFactory : IProfilerFactory
{
  public static readonly NullProfilerFactory Instance = new();

  public IProfiler GetProfiler(string category)
  {
    return new NullProfiler();
  }

  /// <summary>A no-op <see cref="IProfiler"/>.</summary>
  private sealed class NullProfiler : IProfiler
  {
    public ProfilingScope Track(string task)                  => default;
    public ProfilingScope Track(string category, string task) => default;
  }
}
