namespace Surreal.Graphics;

/// <summary>
/// A scope for denoting a debug region in the graphics backend.
/// </summary>
public readonly struct GraphicsDebugScope : IDisposable
{
  private readonly IGraphicsBackend _backend;

  public GraphicsDebugScope(IGraphicsBackend backend, string name)
  {
    _backend = backend;

    backend.BeginDebugScope(name);
  }

  public void Dispose()
  {
    _backend.EndDebugScope();
  }
}
