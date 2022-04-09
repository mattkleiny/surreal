using Surreal.Mathematics;
using Surreal.Timing;

namespace Surreal.Automata.BehaviourTrees.Tasks;

public class RandomDelayTests
{
  [Test]
  public void it_should_wait_a_random_amount_of_time()
  {
    var timeRange = new TimeSpanRange(1.Seconds(), 4.Seconds());
    var timeStep = timeRange.Max / 4f;

    var tree = new BehaviourTree(this, new RandomDelay(timeRange));

    for (int i = 0; i < 5; i++)
    {
      if (tree.Update(timeStep) == BehaviourStatus.Success)
      {
        Assert.Pass();
      }
    }

    Assert.Fail();
  }
}
