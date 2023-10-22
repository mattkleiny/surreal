namespace Surreal.Graphics.Rendering;

/// <summary>
/// Allows for context-specific rendering.
/// </summary>
public interface IRenderContext
{
  void OnBeginFrame(in RenderFrame frame);
  void OnEndFrame(in RenderFrame frame);
}

/// <summary>
/// Convenience class for <see cref="IRenderContext"/>.
/// </summary>
public abstract class RenderContext : IRenderContext, IDisposable
{
  protected internal virtual void OnBeginFrame(in RenderFrame frame)
  {
  }

  protected internal virtual void OnEndFrame(in RenderFrame frame)
  {
  }

  protected internal virtual void Dispose()
  {
  }

  void IRenderContext.OnBeginFrame(in RenderFrame frame)
  {
    OnBeginFrame(in frame);
  }

  void IRenderContext.OnEndFrame(in RenderFrame frame)
  {
    OnEndFrame(in frame);
  }

  void IDisposable.Dispose()
  {
    Dispose();
  }
}
