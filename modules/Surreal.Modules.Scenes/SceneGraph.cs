using Surreal.Graphics.Rendering;

namespace Surreal;

/// <summary>
/// A graph of <see cref="SceneNode"/>s.
/// </summary>
public sealed class SceneGraph : IDisposable
{
  /// <summary>
  /// The root node of the graph.
  /// </summary>
  public SceneNode Root { get; } = new();

  public void Enable()
  {
    Root.OnEnable();
  }

  public void Disable()
  {
    Root.OnDisable();
  }

  public void Update()
  {
    Root.OnUpdate();
  }

  public void Render(in RenderFrame frame, IRenderContextManager manager)
  {
    Root.OnRender(in frame, manager);
  }

  public void Dispose()
  {
    Root.Dispose();
  }
}
