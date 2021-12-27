namespace Surreal.Diagnostics.Profiling;

public sealed class NullProfilerFactory : IProfilerFactory
{
	public static readonly NullProfilerFactory Instance = new();

	public IProfiler GetProfiler(string category)
	{
		return new NullProfiler();
	}

	private sealed class NullProfiler : IProfiler
	{
		public ProfilingScope Track(string task) => default;
		public ProfilingScope Track(string category, string task) => default;
	}
}
