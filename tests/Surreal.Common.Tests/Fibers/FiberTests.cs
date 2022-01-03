using NUnit.Framework;

namespace Surreal.Fibers;

public class FiberTests
{
  [Test]
  public void it_should_execute_and_yield()
  {
    var executions = 0;

    FiberTask.Create(async () =>
    {
      executions++;
      await FiberTask.Yield();
      executions++;
    });

    Assert.AreEqual(1, executions);
    FiberScheduler.Tick();
    Assert.AreEqual(2, executions);
  }
}
