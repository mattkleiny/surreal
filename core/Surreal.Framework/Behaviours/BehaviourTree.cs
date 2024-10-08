﻿using Surreal.Timing;

namespace Surreal.Behaviours;

/// <summary>
/// Status for execution of a <see cref="BehaviourNode"/>.
/// </summary>
public enum BehaviourStatus
{
  Sleeping,
  Running,
  Success,
  Failure
}

/// <summary>
/// The context for <see cref="BehaviourNode"/> operations.
/// </summary>
public readonly record struct BehaviourContext(object Owner, BehaviourTree BehaviourTree);

/// <summary>
/// An behaviour tree.
/// </summary>
public sealed class BehaviourTree(object owner) : BehaviourNode
{
  /// <summary>
  /// The root node of the <see cref="BehaviourTree"/>.
  /// </summary>
  public required BehaviourNode Root { get; init; }

  /// <summary>
  /// Updates the <see cref="BehaviourTree"/>.
  /// </summary>
  public BehaviourStatus Update(DeltaTime deltaTime)
  {
    var context = new BehaviourContext(owner, this);

    return base.Update(context, deltaTime);
  }

  protected override BehaviourStatus OnUpdate(in BehaviourContext context, DeltaTime deltaTime)
  {
    return Root.Update(in context, deltaTime);
  }
}
