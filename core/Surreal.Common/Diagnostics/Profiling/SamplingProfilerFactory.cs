namespace Surreal.Diagnostics.Profiling;

/// <summary>
/// A sampler for <see cref="IProfiler" /> operations.
/// </summary>
public interface IProfileSampler
{
  void Sample(string category, string task, TimeSpan duration);
}

/// <summary>
/// A <see cref="IProfilerFactory" /> that builds <see cref="SamplingProfiler" />s.
/// </summary>
public sealed class SamplingProfilerFactory(IProfileSampler sampler) : IProfilerFactory
{
  public IProfiler GetProfiler(string category)
  {
    return new SamplingProfiler(sampler, category);
  }

  /// <summary>
  /// A <see cref="IProfiler" /> for <see cref="SamplingProfiler" />s.
  /// </summary>
  private sealed class SamplingProfiler(IProfileSampler sampler, string category) : IProfiler
  {
    public ProfilingScope Track(string task)
    {
      return Track(category, task);
    }

    public ProfilingScope Track(string category, string task)
    {
      return new ProfilingScope(category, task, sampler);
    }
  }
}
