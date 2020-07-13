namespace Surreal.Diagnostics.Profiling {
  public interface IProfiler {
    ProfilingScope Track(string task);
    ProfilingScope Track(string category, string task);
  }
}