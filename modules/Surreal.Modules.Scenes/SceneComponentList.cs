using Surreal.Collections;

namespace Surreal;

/// <summary>
/// A list of <see cref="ISceneComponent"/>s.
/// </summary>
public sealed class SceneComponentList(ISceneNode node) : Collection<ISceneComponent>
{
  protected override void OnItemAdded(ISceneComponent item)
  {
    item.OnAttach(node);

    base.OnItemAdded(item);
  }

  protected override void OnItemRemoved(ISceneComponent item)
  {
    item.OnDetach(node);

    base.OnItemRemoved(item);
  }
}
