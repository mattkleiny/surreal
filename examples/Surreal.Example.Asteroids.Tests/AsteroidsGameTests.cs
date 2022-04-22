namespace Asteroids;

public class AsteroidsGameTests : GameTestCase<AsteroidsGame>
{
  [Test]
  public void it_should_bootstrap_and_tick()
  {
    GameUnderTest.Run(1.Seconds());
  }
}
