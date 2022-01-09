using Prelude;

namespace Surreal;

public class PreludeGameTests : GameTestCase<PreludeGame>
{
  [Test]
  public async Task it_should_bootstrap_and_tick()
  {
    await GameUnderTest.RunAsync(1.Seconds());
  }
}
