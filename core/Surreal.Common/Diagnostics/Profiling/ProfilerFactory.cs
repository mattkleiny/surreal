using System;
using Surreal.Text;

namespace Surreal.Diagnostics.Profiling {
  public interface IProfilerFactory {
    IProfiler GetProfiler(string category);
  }

  public static class ProfilerFactory {
    public static IProfilerFactory Current { get; set; } = NullProfilerFactory.Instance;

    public static IProfiler GetProfiler<T>()             => GetProfiler(typeof(T));
    public static IProfiler GetProfiler(Type type)       => GetProfiler(type.GetFullNameWithoutGenerics());
    public static IProfiler GetProfiler(string category) => new LazyProfiler(category);

    private sealed class LazyProfiler : IProfiler {
      private readonly Lazy<IProfiler> profiler;

      public LazyProfiler(string category) => profiler = new Lazy<IProfiler>(() => Current.GetProfiler(category));

      public ProfilingScope Track(string task)                  => profiler.Value.Track(task);
      public ProfilingScope Track(string category, string task) => profiler.Value.Track(category, task);
    }
  }
}