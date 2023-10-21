using Surreal.Resources;

namespace Surreal.Graphics.Rendering;

/// <summary>
/// Allows for context-specific rendering.
/// </summary>
public interface IRenderContext : IDisposable
{
  void OnBeginFrame(in RenderFrame frame);
  void OnEndFrame(in RenderFrame frame);

  void OnBeginUse(in RenderFrame frame);
  void OnEndUse(in RenderFrame frame);
}

/// <summary>
/// Describes how to build a <see cref="IRenderContext"/>.
/// </summary>
public interface IRenderContextDescriptor
{
  Task<IRenderContext> BuildContextAsync(GraphicsContext context, IResourceManager resources, CancellationToken cancellationToken = default);
}

/// <summary>
/// Convenience class for <see cref="IRenderContext"/>.
/// </summary>
public abstract class RenderContext : IRenderContext
{
  public virtual void OnBeginFrame(in RenderFrame frame)
  {
  }

  public virtual void OnEndFrame(in RenderFrame frame)
  {
  }

  public virtual void OnBeginUse(in RenderFrame frame)
  {
  }

  public virtual void OnEndUse(in RenderFrame frame)
  {
  }

  public virtual void Dispose()
  {
  }
}
