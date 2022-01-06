using Headless;

namespace Surreal;

public class HeadlessGameTests : GameTestCase<HeadlessGame>
{
  [Test]
  public void it_should_bootstrap_and_tick()
  {
    GameUnderTest.Tick(16.Milliseconds());
  }
}
