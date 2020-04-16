namespace Surreal.Diagnostics.Profiling
{
  public interface IProfilerFactory
  {
    IProfiler GetProfiler(string category);
  }
}
