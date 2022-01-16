using Surreal.Collections;
using Surreal.Timing;
using Surreal.Utilities;

namespace Surreal.Automata.BehaviourTrees.Decorators;

public class ComparePropertyTests
{
  public static Property<int> TestProperty { get; } = new(nameof(TestProperty));

  [Test, AutoFixture]
  public void it_should_return_success_regardless_of_child(BehaviourNode childNode)
  {
    var tree     = new BehaviourTree(this, new CompareProperty<int>(childNode, TestProperty, new ValueComparison<int>(ComparisonType.GreaterThanOrEqual, 5)));
    var timeStep = 0.25f.Seconds();

    tree.Update(timeStep);
    childNode.Received(0).OnUpdate(Arg.Any<BehaviourContext>(), timeStep);

    tree.Properties.Set(TestProperty, 5);

    tree.Update(timeStep);
    childNode.Received(1).OnUpdate(Arg.Any<BehaviourContext>(), timeStep);
  }
}
