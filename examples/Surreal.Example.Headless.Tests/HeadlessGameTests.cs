namespace Headless;

public class HeadlessGameTests : GameTestCase<HeadlessGame>
{
  [Test]
  public void it_should_bootstrap_and_tick()
  {
    GameUnderTest.Run(1.Seconds());
  }
}
