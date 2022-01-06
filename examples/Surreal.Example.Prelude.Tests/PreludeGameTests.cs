using Prelude;

namespace Surreal;

public class PreludeGameTests : GameTestCase<PreludeGame>
{
  [Test]
  public void it_should_bootstrap_and_tick()
  {
    GameUnderTest.Tick(16.Milliseconds());
  }
}
