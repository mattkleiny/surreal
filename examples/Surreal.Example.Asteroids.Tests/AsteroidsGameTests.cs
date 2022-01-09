using Asteroids;

namespace Surreal;

public class AsteroidsGameTests : GameTestCase<AsteroidsGame>
{
  [Test]
  public async Task it_should_bootstrap_and_tick()
  {
    await GameUnderTest.RunAsync(1.Seconds());
  }
}
