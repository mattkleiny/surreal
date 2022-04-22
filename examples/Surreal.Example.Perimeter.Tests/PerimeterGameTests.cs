namespace Perimeter;

public class PerimeterGameTests : GameTestCase<PerimeterGame>
{
  [Test]
  public void it_should_bootstrap_and_tick()
  {
    GameUnderTest.Run(1.Seconds());
  }
}
