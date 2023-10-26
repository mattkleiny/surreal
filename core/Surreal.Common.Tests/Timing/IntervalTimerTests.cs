namespace Surreal.Timing;

public class IntervalTimerTests
{
  [Test]
  public void it_should_tick_and_evaluate_time()
  {
    var timer = new IntervalTimer(TimeSpan.FromSeconds(1));

    timer.Tick(TimeSpan.FromSeconds(0.25)).Should().BeFalse();
    timer.Tick(TimeSpan.FromSeconds(0.25)).Should().BeFalse();
    timer.Tick(TimeSpan.FromSeconds(0.25)).Should().BeFalse();
    timer.Tick(TimeSpan.FromSeconds(0.25)).Should().BeTrue();
    timer.Tick(TimeSpan.FromSeconds(0.25)).Should().BeTrue();

    timer.Reset();

    timer.Tick(TimeSpan.FromSeconds(0.25)).Should().BeFalse();
    timer.Tick(TimeSpan.FromSeconds(0.25)).Should().BeFalse();
    timer.Tick(TimeSpan.FromSeconds(0.25)).Should().BeFalse();
    timer.Tick(TimeSpan.FromSeconds(0.25)).Should().BeTrue();
  }
}
