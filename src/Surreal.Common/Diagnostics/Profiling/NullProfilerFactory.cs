﻿namespace Surreal.Diagnostics.Profiling
{
  public sealed class NullProfilerFactory : IProfilerFactory
  {
    public static readonly NullProfilerFactory Instance = new();

    public IProfiler GetProfiler(string category)
    {
      return NullProfiler.Instance;
    }

    private sealed class NullProfiler : IProfiler
    {
// ReSharper disable once MemberHidesStaticFromOuterClass
      public static readonly NullProfiler Instance = new();

      public ProfilingScope Track(string task)                  => ProfilingScope.Null;
      public ProfilingScope Track(string category, string task) => ProfilingScope.Null;
    }
  }
}