namespace Isaac;

public class IsaacGameTests : GameTestCase<IsaacGame>
{
  [Test]
  public void it_should_bootstrap_and_tick()
  {
    GameUnderTest.Run(1.Seconds());
  }
}
