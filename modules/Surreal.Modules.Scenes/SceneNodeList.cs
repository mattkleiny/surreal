using Surreal.Collections;

namespace Surreal;

/// <summary>
/// A collection of <see cref="SceneNode"/>s.
/// </summary>
public sealed class SceneNodeList(ISceneNode node) : Collection<ISceneNode>
{
  protected override void OnItemAdded(ISceneNode item)
  {
    item.OnParented(node);

    base.OnItemAdded(item);
  }

  protected override void OnItemRemoved(ISceneNode item)
  {
    item.OnUnparented();

    base.OnItemRemoved(item);
  }
}
