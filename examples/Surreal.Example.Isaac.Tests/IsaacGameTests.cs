using Isaac;

namespace Surreal;

public class IsaacGameTests : GameTestCase<IsaacGame>
{
  [Test]
  public async Task it_should_bootstrap_and_tick()
  {
    await GameUnderTest.RunAsync(1.Seconds());
  }
}
