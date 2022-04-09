using Surreal.Timing;

namespace Surreal.Diagnostics.Profiling;

/// <summary>A component capable of recording profiling information.</summary>
public interface IProfiler
{
  ProfilingScope Track(string task);
  ProfilingScope Track(string category, string task);
}

/// <summary>A scope for <see cref="IProfiler"/> operations.</summary>
public readonly struct ProfilingScope : IDisposable
{
  private readonly string category;
  private readonly string task;
  private readonly IProfileSampler? sampler;
  private readonly TimeStamp startTime;

  public ProfilingScope(string category, string task, IProfileSampler? sampler)
  {
    this.category = category;
    this.task = task;
    this.sampler = sampler;

    startTime = TimeStamp.Now;
  }

  public void Dispose()
  {
    var endTime = TimeStamp.Now;

    sampler?.Sample(category, task, endTime - startTime);
  }
}
