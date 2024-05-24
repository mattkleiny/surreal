namespace Surreal.BehaviourTrees;

/// <summary>
/// Represent a <see cref="BehaviourNode"/> that implements some composite.
/// </summary>
public abstract class BehaviourComposite : BehaviourNode
{
  /// <summary>
  /// The children of the <see cref="BehaviourComposite"/>.
  /// </summary>
  public List<BehaviourNode> Children { get; init; } = [];
}
