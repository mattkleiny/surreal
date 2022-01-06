using Asteroids;

namespace Surreal;

public class AsteroidsGameTests : GameTestCase<AsteroidsGame>
{
  [Test]
  public void it_should_bootstrap_and_tick()
  {
    GameUnderTest.Tick(16.Milliseconds());
  }
}
