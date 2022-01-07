using Headless;

namespace Surreal;

public class HeadlessGameTests : GameTestCase<HeadlessGame>
{
  [Test]
  public async Task it_should_bootstrap_and_tick()
  {
    await GameUnderTest.RunAsync(1.Seconds());
  }
}
