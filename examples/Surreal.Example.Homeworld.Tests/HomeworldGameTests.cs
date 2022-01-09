namespace Homeworld;

public class HomeworldGameTests : GameTestCase<HomeworldGame>
{
  [Test]
  public async Task it_should_bootstrap_and_tick()
  {
    await GameUnderTest.RunAsync(1.Seconds());
  }
}
