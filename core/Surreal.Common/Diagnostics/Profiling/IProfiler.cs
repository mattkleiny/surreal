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
public readonly struct ProfilingScope : IDisposable
{
  private readonly string _category;
  private readonly string _task;
  private readonly IProfileSampler? _sampler;
  private readonly TimeStamp _startTime;

  public ProfilingScope(string category, string task, IProfileSampler? sampler)
  {
    _category = category;
    _task = task;
    _sampler = sampler;

    _startTime = TimeStamp.Now;
  }

  public void Dispose()
  {
    var endTime = TimeStamp.Now;

    _sampler?.Sample(_category, _task, endTime - _startTime);
  }
}
