﻿using Surreal.Collections;
using Surreal.Timing;

namespace Surreal.Automata.BehaviourTrees.Decorators;

public class CheckPropertyTests
{
  public static Property<bool> TestProperty { get; } = new(nameof(TestProperty));

  [Test, AutoFixture]
  public void it_should_return_success_regardless_of_child(BehaviourNode childNode)
  {
    var properties = new PropertyCollection();
    var tree       = new BehaviourTree(this, properties, new CheckProperty<bool>(childNode, TestProperty, true));
    var timeStep   = 0.25f.Seconds();

    tree.Update(timeStep);
    childNode.Received(0).OnUpdate(tree.Context, timeStep);

    properties.Set(TestProperty, true);

    tree.Update(timeStep);
    childNode.Received(1).OnUpdate(tree.Context, timeStep);
  }
}