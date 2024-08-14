namespace Surreal.Graphics;

/// <summary>
/// A scope for denoting a debug region in the graphics backend.
/// </summary>
public readonly struct GraphicsDebugScope : IDisposable
{
  private readonly IGraphicsDevice _device;

  public GraphicsDebugScope(IGraphicsDevice device, string name)
  {
    _device = device;

    device.BeginDebugScope(name);
  }

  public void Dispose()
  {
    _device.EndDebugScope();
  }
}
