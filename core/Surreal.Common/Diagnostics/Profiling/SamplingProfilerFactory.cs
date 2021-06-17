using System;

namespace Surreal.Diagnostics.Profiling {
  public interface IProfileSampler {
    void Sample(string category, string task, TimeSpan duration);
  }

  public sealed class SamplingProfilerFactory : IProfilerFactory {
    private readonly IProfileSampler sampler;

    public SamplingProfilerFactory(IProfileSampler sampler) {
      this.sampler = sampler;
    }

    public IProfiler GetProfiler(string category) {
      return new SamplingProfiler(sampler, category);
    }

    private sealed class SamplingProfiler : IProfiler {
      private readonly IProfileSampler sampler;
      private readonly string          category;

      public SamplingProfiler(IProfileSampler sampler, string category) {
        this.sampler  = sampler;
        this.category = category;
      }

      public ProfilingScope Track(string task)                  => Track(category, task);
      public ProfilingScope Track(string category, string task) => new(category, task, sampler);
    }
  }
}