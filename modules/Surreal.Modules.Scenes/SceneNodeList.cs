using Surreal.Collections;

namespace Surreal;

/// <summary>
/// A collection of <see cref="SceneNode"/>s.
/// </summary>
public sealed class SceneNodeList : Collection<ISceneNode>
{
  private readonly ISceneNode _node;

  public SceneNodeList(ISceneNode node)
  {
    _node = node;
  }

  protected override void OnItemAdded(ISceneNode item)
  {
    item.OnParented(_node);

    base.OnItemAdded(item);
  }

  protected override void OnItemRemoved(ISceneNode item)
  {
    item.OnUnparented();

    base.OnItemRemoved(item);
  }
}
