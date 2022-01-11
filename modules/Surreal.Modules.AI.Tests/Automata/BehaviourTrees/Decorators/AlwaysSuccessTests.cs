using Surreal.Timing;

namespace Surreal.Automata.BehaviourTrees.Decorators;

public class AlwaysSuccessTests
{
  [Test, AutoFixture]
  public void it_should_return_success_regardless_of_child(BehaviourNode childNode)
  {
    childNode.OnUpdate(Arg.Any<BehaviourContext>(), Arg.Any<DeltaTime>()).Returns(BehaviourStatus.Failure);

    var tree     = new BehaviourTree(this, new AlwaysSuccess(childNode));
    var timeStep = 0.25f.Seconds();

    Assert.AreEqual(BehaviourStatus.Success, tree.Update(timeStep));
  }
}
