using System;

namespace Surreal.Diagnostics.Profiling
{
  public interface IProfileSampler
  {
    void Sample(string category, string task, TimeSpan duration);
  }
}
