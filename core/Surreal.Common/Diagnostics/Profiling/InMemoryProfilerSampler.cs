﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Surreal.Collections;
using Surreal.Data;
using Surreal.Data.VFS;
using Surreal.Diagnostics.Logging;
using Path = Surreal.Data.VFS.Path;

namespace Surreal.Diagnostics.Profiling {
  public sealed class InMemoryProfilerSampler : IProfileSampler {
    private static readonly ILog Log = LogFactory.GetLog<InMemoryProfilerSampler>();

    public InMemoryProfilerSampler(int sampleCount = 30) {
      Debug.Assert(sampleCount > 0, "sampleCount > 0");

      Samplers = new SamplerCollection(sampleCount);
    }

    public SamplerCollection Samplers { get; }

    public void Sample(string category, string task, TimeSpan duration) {
      Samplers.GetSampler(category, task).Record(duration);
    }

    public async Task ExportToCSVAsync(Path path) {
      await using var stream = await path.OpenOutputStreamAsync();
      await using var writer = new StreamWriter(stream, Encoding.UTF8);

      Log.Trace($"Exporting profiler report to {path.ToString()}");

      await writer.WriteLineAsync("Category,Task,Maximum(ms),Minimum(ms),Average(ms)");

      foreach (var sampler in Samplers) {
        var maximum = sampler.Maximum.TotalMilliseconds;
        var minimum = sampler.Minimum.TotalMilliseconds;
        var average = sampler.Average.TotalMilliseconds;

        await writer.WriteLineAsync($"{sampler.Category},{sampler.Task},{maximum.ToString("F")},{minimum.ToString("F")},{average.ToString("F")}");
      }
    }

    public sealed class Sampler : IEnumerable<TimeSpan> {
      private readonly RingBuffer<TimeSpan> samples;

      public Sampler(string category, string task, int sampleCount) {
        Category = category;
        Task     = task;

        samples = new RingBuffer<TimeSpan>(sampleCount);
      }

      public string Category { get; }
      public string Task     { get; }

      public TimeSpan Maximum => samples.FastMax();
      public TimeSpan Minimum => samples.FastMin();
      public TimeSpan Average => samples.FastAverage();

      public void Record(TimeSpan duration) {
        // serialized write access; unguarded read access.
        // we want to avoid race conditions in the RingBuffer as it's internally unguarded.
        // however, concurrent read access to samples is not a huge concern and if they
        // are out of sync between threads it's less relevant.
        lock (samples) {
          samples.Add(duration);
        }
      }

      public RingBuffer<TimeSpan>.Enumerator      GetEnumerator() => samples.GetEnumerator();
      IEnumerator<TimeSpan> IEnumerable<TimeSpan>.GetEnumerator() => GetEnumerator();
      IEnumerator IEnumerable.                    GetEnumerator() => GetEnumerator();
    }

    public sealed class SamplerCollection : IEnumerable<Sampler> {
      private readonly ConcurrentDictionary<string, Sampler> samplers = new(StringComparer.OrdinalIgnoreCase);

      private readonly int sampleCount;

      public SamplerCollection(int sampleCount) {
        Debug.Assert(sampleCount > 0, "sampleCount > 0");

        this.sampleCount = sampleCount;
      }

      public Sampler GetSampler(string category, string task) {
        var key = $"{category}:{task}";

        if (!samplers.TryGetValue(key, out var sampler)) {
          sampler = new Sampler(category, task, sampleCount);
          samplers.TryAdd(key, sampler);
        }

        return sampler;
      }

      public IEnumerator<Sampler> GetEnumerator() => samplers.Values.GetEnumerator();
      IEnumerator IEnumerable.    GetEnumerator() => GetEnumerator();
    }
  }
}