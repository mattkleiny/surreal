using Surreal.Collections;

namespace Surreal.Scenes;

/// <summary>
/// A collection of <see cref="ISceneComponent"/>s.
/// </summary>
public sealed class SceneComponentCollection : Collection<ISceneComponent>
{
  private readonly ISceneNode _node;

  public SceneComponentCollection(ISceneNode node)
  {
    _node = node;
  }

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
