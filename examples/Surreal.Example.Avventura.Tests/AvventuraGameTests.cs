namespace Avventura;

public class AvventuraGameTests : GameTestCase<AvventuraGame>
{
  [Test]
  public void it_should_bootstrap_and_tick()
  {
    GameUnderTest.Run(1.Seconds());
  }
}
