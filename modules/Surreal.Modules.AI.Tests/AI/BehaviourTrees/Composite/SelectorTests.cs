using Surreal.Timing;

namespace Surreal.AI.BehaviourTrees.Composite;

public class SelectorTests
{
  [Test, AutoFixture]
  public void it_should_select_first_non_failing_node(BehaviourNode failingNode, BehaviourNode successfulNode)
  {
    failingNode.OnUpdate(Arg.Any<BehaviourContext>(), Arg.Any<DeltaTime>()).Returns(BehaviourStatus.Failure);
    successfulNode.OnUpdate(Arg.Any<BehaviourContext>(), Arg.Any<DeltaTime>()).Returns(BehaviourStatus.Success);

    var tree = new BehaviourTree(this, new Selector(failingNode, successfulNode));
    var timeStep = 0.25f.Seconds();

    tree.Update(timeStep).Should().Be(BehaviourStatus.Success);

    failingNode.Received(1).OnUpdate(Arg.Any<BehaviourContext>(), timeStep);
    successfulNode.Received(1).OnUpdate(Arg.Any<BehaviourContext>(), timeStep);
  }
}
