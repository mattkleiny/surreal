using System.Threading.Tasks;
using Surreal.Fibers;
using Xunit;

namespace Surreal.Core.Fibers
{
  public class FiberTests
  {
    [Fact]
    public void it_should_execute_and_yield()
    {
      var scheduler  = new FiberScheduler();
      var executions = 0;

      Fiber.Start(scheduler, async () =>
      {
        executions++;
        await Task.Yield();
        executions++;
      });

      scheduler.Run();
      Assert.Equal(1, executions);
      scheduler.Run();
      Assert.Equal(2, executions);
    }
  }
}