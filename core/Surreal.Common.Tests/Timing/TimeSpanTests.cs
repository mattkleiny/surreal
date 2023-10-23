using Surreal.Timing;

namespace Surreal.Common.Tests.Timing;

public class TimeSpanTests
{
  [Test]
  public async Task it_should_capture_a_timing_snapshot_and_convert_to_time_span()
  {
    var start = TimeStamp.Now;

    await Task.Delay(TimeSpan.FromSeconds(1));

    var stop = TimeStamp.Now;
    var timeSpan = stop - start;

    timeSpan.Should().BeGreaterOrEqualTo(TimeSpan.FromMilliseconds(500));
  }
}
