using System;

namespace Surreal.Mathematics.Timing {
  public static class TimeSpans {
    public static TimeSpan Milliseconds(this int interval) => TimeSpan.FromMilliseconds(interval);
    public static TimeSpan Seconds(this int interval)      => TimeSpan.FromSeconds(interval);
    public static TimeSpan Minutes(this int interval)      => TimeSpan.FromMinutes(interval);
    public static TimeSpan Hours(this int interval)        => TimeSpan.FromHours(interval);
    public static TimeSpan Days(this int interval)         => TimeSpan.FromDays(interval);
  }
}