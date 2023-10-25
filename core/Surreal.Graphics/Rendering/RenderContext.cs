namespace Surreal.Graphics.Rendering;

/// <summary>
/// Convenience class for <see cref="RenderContext"/>.
/// </summary>
public abstract class RenderContext : IDisposable
{
  public virtual void OnBeginFrame(in RenderFrame frame)
  {
  }

  public virtual void OnEndFrame(in RenderFrame frame)
  {
  }

  public virtual void OnBeginViewport(in RenderFrame frame, IRenderViewport viewport)
  {
  }

  public virtual void OnEndViewport(in RenderFrame frame, IRenderViewport viewport)
  {
  }

  public virtual void OnBeginPass(in RenderFrame frame, IRenderViewport viewport)
  {
  }

  public virtual void OnEndPass(in RenderFrame frame, IRenderViewport viewport)
  {
  }

  public virtual void Dispose()
  {
  }
}
