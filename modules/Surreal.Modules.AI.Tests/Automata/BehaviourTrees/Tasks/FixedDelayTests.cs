using Surreal.Timing;

namespace Surreal.Automata.BehaviourTrees.Tasks;

public class FixedDelayTests
{
  [Test]
  public void it_should_wait_a_fixed_amount_of_time()
  {
    var tree = new BehaviourTree(this, new FixedDelay(1.Seconds()));
    var timeStep = 0.25f.Seconds();

    tree.Update(timeStep).Should().Be(BehaviourStatus.Running);
    tree.Update(timeStep).Should().Be(BehaviourStatus.Running);
    tree.Update(timeStep).Should().Be(BehaviourStatus.Running);
    tree.Update(timeStep).Should().Be(BehaviourStatus.Success);
  }
}
