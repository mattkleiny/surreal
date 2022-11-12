﻿using Surreal.Timing;

namespace Surreal.Automata.BehaviourTrees.Decorators;

public class AlwaysSuccessTests
{
  [AutoTest]
  public void it_should_return_success_regardless_of_child(BehaviourNode childNode)
  {
    childNode.OnUpdate(Arg.Any<BehaviourContext>(), Arg.Any<TimeDelta>()).Returns(BehaviourStatus.Failure);

    var tree = new BehaviourTree(this, new AlwaysSuccess(childNode));
    var timeStep = 0.25f.Seconds();

    tree.Update(timeStep).Should().Be(BehaviourStatus.Success);
  }
}

