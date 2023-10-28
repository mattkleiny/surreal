﻿using Surreal.Collections;
using Surreal.Diagnostics.Logging;
using Surreal.IO;

namespace Surreal.Diagnostics.Profiling;

/// <summary>
/// A <see cref="IProfileSampler" /> that records profiling results to an in-memory buffer.
/// </summary>
public sealed class InMemoryProfilerSampler : IProfileSampler
{
  private static readonly ILog Log = LogFactory.GetLog<InMemoryProfilerSampler>();

  public InMemoryProfilerSampler(int sampleCount = 30)
  {
    Debug.Assert(sampleCount > 0, "sampleCount > 0");

    Samplers = new SamplerCollection(sampleCount);
  }

  public SamplerCollection Samplers { get; }

  public void Sample(string category, string task, TimeSpan duration)
  {
    Samplers.GetSampler(category, task).Record(duration);
  }

  /// <summary>
  /// Exports the results to the given CSV file.
  /// </summary>
  public async ValueTask ExportToCsvAsync(VirtualPath path)
  {
    await using var writer = path.OpenOutputStreamWriter();

    Log.Trace($"Exporting profiler report to {path}");

    await writer.WriteLineAsync("Category,Task,Maximum(ms),Minimum(ms),Average(ms)");

    foreach (var sampler in Samplers)
    {
      var maximum = sampler.Maximum.TotalMilliseconds;
      var minimum = sampler.Minimum.TotalMilliseconds;
      var average = sampler.Average.TotalMilliseconds;

      await writer.WriteLineAsync($"{sampler.Category},{sampler.Task},{maximum:F},{minimum:F},{average:F}");
    }
  }

  /// <summary>
  /// A sampler that records details about an operation.
  /// </summary>
  public sealed class Sampler(string category, string task, int sampleCount) : IEnumerable<TimeSpan>
  {
    private readonly RingBuffer<TimeSpan> _samples = new(sampleCount);

    public string Category { get; } = category;
    public string Task { get; } = task;

    public TimeSpan Maximum => _samples.FastMax();
    public TimeSpan Minimum => _samples.FastMin();
    public TimeSpan Average => _samples.FastAverage();

    IEnumerator<TimeSpan> IEnumerable<TimeSpan>.GetEnumerator()
    {
      return GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public void Record(TimeSpan duration)
    {
      // serialized write access; unguarded read access.
      // we want to avoid race conditions in the RingBuffer as it's internally unguarded.
      // however, concurrent read access to samples is not a huge concern and if they
      // are out of sync between threads it's less relevant.
      lock (_samples)
      {
        _samples.Add(duration);
      }
    }

    public RingBuffer<TimeSpan>.Enumerator GetEnumerator()
    {
      return _samples.GetEnumerator();
    }
  }

  /// <summary>
  /// A collection of <see cref="Sampler" />s.
  /// </summary>
  public sealed class SamplerCollection : IEnumerable<Sampler>
  {
    private readonly int _sampleCount;
    private readonly ConcurrentDictionary<(string Category, string Task), Sampler> _samplers = new();

    public SamplerCollection(int sampleCount)
    {
      Debug.Assert(sampleCount > 0, "sampleCount > 0");

      _sampleCount = sampleCount;
    }

    public IEnumerator<Sampler> GetEnumerator()
    {
      return _samplers.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public Sampler GetSampler(string category, string task)
    {
      return _samplers.GetOrAdd((category, task), CreateSampler);
    }

    private Sampler CreateSampler((string Category, string Task) key)
    {
      return new Sampler(key.Category, key.Task, _sampleCount);
    }
  }
}
