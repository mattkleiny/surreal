using Surreal.Timing;

namespace Surreal.Common.Tests.Timing;

public class DeltaTimeTests
{
  [Test]
  public void it_should_convert_automatically_from_time_span()
  {
    DeltaTime deltaTime = TimeSpan.FromSeconds(1);

    deltaTime.Seconds.Should().Be(1f);

    TimeSpan value = deltaTime;

    value.Should().Be(TimeSpan.FromSeconds(1f));
  }

  [Test]
  public void it_should_convert_automatically_to_and_from_float()
  {
    DeltaTime deltaTime = 1f;

    deltaTime.Seconds.Should().Be(1f);

    float value = deltaTime;

    value.Should().Be(1f);
  }
}
