using Headless;
using Surreal.Memory;

namespace Surreal;

public class HeadlessGameTests : GameTestCase<HeadlessGame>
{
  [Test]
  public async Task it_should_bootstrap_and_tick()
  {
    await GameUnderTest.RunAsync(1.Seconds());
  }

  [Test, DotMemoryUnit(CollectAllocations = true, FailIfRunWithoutSupport = false)]
  public void it_should_not_allocate_memory_in_core_loop()
  {
    var checkpoint = dotMemory.Check();

    GameUnderTest.Tick(16.Milliseconds());

    dotMemory.Check(memory =>
    {
      var size = new Size(memory.GetTrafficFrom(checkpoint).AllocatedMemory.SizeInBytes);

      Assert.That(size, Is.EqualTo(0.Bytes()));
    });
  }
}
