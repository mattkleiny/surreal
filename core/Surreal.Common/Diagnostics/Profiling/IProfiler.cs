using Surreal.Timing;

namespace Surreal.Diagnostics.Profiling;

/// <summary>
/// A component capable of recording profiling information.
/// </summary>
public interface IProfiler
{
  ProfilingScope Track(string task);
  ProfilingScope Track(string category, string task);
}

/// <summary>
/// A scope for <see cref="IProfiler" /> operations.
/// </summary>
public readonly struct ProfilingScope(string category, string task, IProfileSampler? sampler) : IDisposable
{
  private readonly TimeStamp _startTime = TimeStamp.Now;

  public void Dispose()
  {
    var endTime = TimeStamp.Now;

    sampler?.Sample(category, task, endTime - _startTime);
  }
}
