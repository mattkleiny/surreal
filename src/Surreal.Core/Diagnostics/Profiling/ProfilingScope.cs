using System;

namespace Surreal.Diagnostics.Profiling {
  public readonly struct ProfilingScope : IDisposable {
    public static readonly ProfilingScope Null = new();

    private readonly string          category;
    private readonly string          task;
    private readonly IProfileSampler sampler;
    private readonly DateTime        startTime;

    public ProfilingScope(string category, string task, IProfileSampler sampler) {
      this.category = category;
      this.task     = task;
      this.sampler  = sampler;

      startTime = DateTime.Now;
    }

    public void Dispose() {
      var endTime = DateTime.Now;

      sampler?.Sample(category, task, endTime - startTime);
    }
  }
}