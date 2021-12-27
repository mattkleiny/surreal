namespace Surreal.Timing;

/// <summary>A range of <see cref="ClockTime"/>s for scheduling.</summary>
public readonly record struct ClockTimeRange(ClockTime Start, ClockTime End)
{
	public static ClockTimeRange Nothing => new(default, default);
	public static ClockTimeRange EntireDay => new(ClockTime.MinValue, ClockTime.MaxValue);

	public bool Contains(ClockTime time)
	{
		return time >= Start && time <= End;
	}
}
