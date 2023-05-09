namespace Surreal.Scenes;

/// <summary>
/// A graph of <see cref="SceneNode"/>s.
/// </summary>
public sealed class SceneGraph : IDisposable
{
  /// <summary>
  /// The root node of the graph.
  /// </summary>
  public SceneNode Root { get; } = new();

  public void OnEnable()
  {
    Root.OnEnable();
  }

  public void OnDisable()
  {
    Root.OnDisable();
  }

  public void OnUpdate()
  {
    Root.OnUpdate();
  }

  public void OnRender()
  {
    Root.OnRender();
  }

  public void Dispose()
  {
    Root.Dispose();
  }
}
