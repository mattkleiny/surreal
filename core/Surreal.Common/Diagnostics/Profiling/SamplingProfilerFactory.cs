namespace Surreal.Diagnostics.Profiling;

/// <summary>A sampler for <see cref="IProfiler"/> operations.</summary>
public interface IProfileSampler
{
  void Sample(string category, string task, TimeSpan duration);
}

/// <summary>A <see cref="IProfilerFactory"/> that builds <see cref="SamplingProfiler"/>s.</summary>
public sealed class SamplingProfilerFactory : IProfilerFactory
{
  private readonly IProfileSampler sampler;

  public SamplingProfilerFactory(IProfileSampler sampler)
  {
    this.sampler = sampler;
  }

  public IProfiler GetProfiler(string category)
  {
    return new SamplingProfiler(sampler, category);
  }

  /// <summary>A <see cref="IProfiler"/> for <see cref="SamplingProfiler"/>s.</summary>
  private sealed class SamplingProfiler : IProfiler
  {
    private readonly IProfileSampler sampler;
    private readonly string category;

    public SamplingProfiler(IProfileSampler sampler, string category)
    {
      this.sampler = sampler;
      this.category = category;
    }

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
