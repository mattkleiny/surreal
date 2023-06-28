using Surreal.Collections;

namespace Surreal;

/// <summary>
/// A list of <see cref="ISceneComponent"/>s.
/// </summary>
public sealed class SceneComponentList(ISceneNode node) : Collection<ISceneComponent>
{
  private readonly ISceneNode _node = node;

  protected override void OnItemAdded(ISceneComponent item)
  {
    item.OnAttach(_node);

    base.OnItemAdded(item);
  }

  protected override void OnItemRemoved(ISceneComponent item)
  {
    item.OnDetach(_node);

    base.OnItemRemoved(item);
  }
}
