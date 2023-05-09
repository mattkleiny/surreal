namespace Surreal.Graphics.Rendering;

/// <summary>
/// Represents a pipeline for graphics rendering.
/// </summary>
public interface IRenderPipeline : IDisposable
{
  void Initialize();
  void OnBeginFrame(in RenderFrame frame);
  void OnEndFrame(in RenderFrame frame);
  void OnBeginCamera(in RenderFrame frame, IRenderCamera camera);
  void OnEndCamera(in RenderFrame frame, IRenderCamera camera);
}

/// <summary>
/// Convenience base class for <see cref="IRenderPipeline"/> implementations.
/// </summary>
public abstract class RenderPipeline : IRenderPipeline
{
  public virtual void Initialize()
  {
  }

  public virtual void OnBeginFrame(in RenderFrame frame)
  {
  }

  public virtual void OnEndFrame(in RenderFrame frame)
  {
  }

  public virtual void OnBeginCamera(in RenderFrame frame, IRenderCamera camera)
  {
  }

  public virtual void OnEndCamera(in RenderFrame frame, IRenderCamera camera)
  {
  }

  public virtual void Dispose()
  {
  }
}
