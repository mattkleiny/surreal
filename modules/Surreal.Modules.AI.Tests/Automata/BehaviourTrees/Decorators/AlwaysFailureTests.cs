using Surreal.Timing;

namespace Surreal.Automata.BehaviourTrees.Decorators;

public class AlwaysFailureTests
{
  [Test, AutoFixture]
  public void it_should_return_failure_regardless_of_child(BehaviourNode childNode)
  {
    childNode.OnUpdate(Arg.Any<BehaviourContext>(), Arg.Any<DeltaTime>()).Returns(BehaviourStatus.Success);

    var tree = new BehaviourTree(this, new AlwaysFailure(childNode));
    var timeStep = 0.25f.Seconds();

    tree.Update(timeStep).Should().Be(BehaviourStatus.Failure);
  }
}
