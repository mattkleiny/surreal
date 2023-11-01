using Surreal.Timing;

namespace Surreal.BehaviourTrees;

/// <summary>
/// A node of a <see cref="BehaviourTree"/>.
/// </summary>
public abstract class BehaviourNode
{
  /// <summary>
  /// The current status of the node.
  /// </summary>
  public BehaviourStatus CurrentStatus { get; private set; }

  /// <summary>
  /// Updates this node.
  /// </summary>
  public BehaviourStatus Update(in BehaviourContext context, DeltaTime deltaTime)
  {
    if (CurrentStatus != BehaviourStatus.Running)
    {
      OnEnter(in context);
    }

    CurrentStatus = OnUpdate(in context, deltaTime);

    if (CurrentStatus != BehaviourStatus.Running)
    {
      OnExit(in context);
    }

    return CurrentStatus;
  }

  /// <summary>
  /// Invoked when the node is updated.
  /// </summary>
  protected abstract BehaviourStatus OnUpdate(in BehaviourContext context, DeltaTime deltaTime);

  /// <summary>
  /// Invoked when the node is entered.
  /// </summary>
  protected virtual void OnEnter(in BehaviourContext context)
  {
  }

  /// <summary>
  /// Invoked when the node is exited.
  /// </summary>
  protected virtual void OnExit(in BehaviourContext context)
  {
  }
}
