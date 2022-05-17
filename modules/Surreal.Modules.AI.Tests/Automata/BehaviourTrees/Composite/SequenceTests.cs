﻿using Surreal.Timing;

namespace Surreal.Automata.BehaviourTrees.Composite;

public class SequenceTests
{
  [AutoTest]
  public void it_should_select_first_non_failing_node(BehaviourNode successfulNode, BehaviourNode failingNode)
  {
    successfulNode.OnUpdate(Arg.Any<BehaviourContext>(), Arg.Any<TimeDelta>()).Returns(BehaviourStatus.Success);
    failingNode.OnUpdate(Arg.Any<BehaviourContext>(), Arg.Any<TimeDelta>()).Returns(BehaviourStatus.Failure);

    var tree = new BehaviourTree(this, new Sequence(successfulNode, failingNode));
    var timeStep = 0.25f.Seconds();

    tree.Update(timeStep).Should().Be(BehaviourStatus.Failure);

    successfulNode.Received(1).OnUpdate(Arg.Any<BehaviourContext>(), timeStep);
    failingNode.Received(1).OnUpdate(Arg.Any<BehaviourContext>(), timeStep);
  }
}