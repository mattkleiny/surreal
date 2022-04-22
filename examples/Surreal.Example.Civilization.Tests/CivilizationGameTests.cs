namespace Civilization;

public class CivilizationGameTests : GameTestCase<CivilizationGame>
{
  [Test]
  public void it_should_bootstrap_and_tick()
  {
    GameUnderTest.Run(1.Seconds());
  }
}
