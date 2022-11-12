namespace Surreal.Timing;

/// <summary>Static extensions for <see cref="TimeSpan" />s.</summary>
public static class TimeSpanExtensions
{
  public static TimeSpan Milliseconds(this int interval)
  {
    return TimeSpan.FromMilliseconds(interval);
  }

  public static TimeSpan Seconds(this int interval)
  {
    return TimeSpan.FromSeconds(interval);
  }

  public static TimeSpan Minutes(this int interval)
  {
    return TimeSpan.FromMinutes(interval);
  }

  public static TimeSpan Hours(this int interval)
  {
    return TimeSpan.FromHours(interval);
  }

  public static TimeSpan Days(this int interval)
  {
    return TimeSpan.FromDays(interval);
  }

  public static TimeSpan Milliseconds(this float interval)
  {
    return TimeSpan.FromMilliseconds(interval);
  }

  public static TimeSpan Seconds(this float interval)
  {
    return TimeSpan.FromSeconds(interval);
  }

  public static TimeSpan Minutes(this float interval)
  {
    return TimeSpan.FromMinutes(interval);
  }

  public static TimeSpan Hours(this float interval)
  {
    return TimeSpan.FromHours(interval);
  }

  public static TimeSpan Days(this float interval)
  {
    return TimeSpan.FromDays(interval);
  }
}



