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
  private readonly IProfileSampler _sampler = sampler;

  public IProfiler GetProfiler(string category)
  {
    return new SamplingProfiler(_sampler, category);
  }

  /// <summary>
  /// A <see cref="IProfiler" /> for <see cref="SamplingProfiler" />s.
  /// </summary>
  private sealed class SamplingProfiler(IProfileSampler sampler, string category) : IProfiler
  {
    private readonly string _category = category;
    private readonly IProfileSampler _sampler = sampler;

    public ProfilingScope Track(string task)
    {
      return Track(_category, task);
    }

    public ProfilingScope Track(string category, string task)
    {
      return new ProfilingScope(category, task, _sampler);
    }
  }
}
