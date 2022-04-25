namespace Surreal.Timing;

/// <summary>Static extensions for <see cref="TimeSpan"/>s.</summary>
public static class TimeSpanExtensions
{
  public static TimeSpan Milliseconds(this int interval) => TimeSpan.FromMilliseconds(interval);
  public static TimeSpan Seconds(this int interval) => TimeSpan.FromSeconds(interval);
  public static TimeSpan Minutes(this int interval) => TimeSpan.FromMinutes(interval);
  public static TimeSpan Hours(this int interval) => TimeSpan.FromHours(interval);
  public static TimeSpan Days(this int interval) => TimeSpan.FromDays(interval);

  public static TimeSpan Milliseconds(this float interval) => TimeSpan.FromMilliseconds(interval);
  public static TimeSpan Seconds(this float interval) => TimeSpan.FromSeconds(interval);
  public static TimeSpan Minutes(this float interval) => TimeSpan.FromMinutes(interval);
  public static TimeSpan Hours(this float interval) => TimeSpan.FromHours(interval);
  public static TimeSpan Days(this float interval) => TimeSpan.FromDays(interval);
}