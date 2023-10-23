namespace Surreal.Graphics.Rendering;

/// <summary>
/// Allows for context-specific rendering.
/// </summary>
public interface IRenderContext
{
  void OnBeginFrame(in RenderFrame frame);
  void OnEndFrame(in RenderFrame frame);

  void OnBeginViewport(in RenderFrame frame, IRenderViewport viewport);
  void OnEndViewport(in RenderFrame frame, IRenderViewport viewport);

  void OnBeginPass(in RenderFrame frame, IRenderViewport viewport);
  void OnEndPass(in RenderFrame frame, IRenderViewport viewport);
}

/// <summary>
/// Convenience class for <see cref="IRenderContext"/>.
/// </summary>
public abstract class RenderContext : IRenderContext, IDisposable
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
