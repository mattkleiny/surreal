using Surreal.Collections;

namespace Surreal.Graphics.Rendering;

/// <summary>
/// A single render pass in a <see cref="RenderPassList"/>.
/// </summary>
public interface IRenderPass : IDisposable
{
  void OnPassAdded();
  void OnPassRemoved();

  void OnBeginFrame(in RenderFrame frame);
  void OnEndFrame(in RenderFrame frame);

  void OnBeginPass(in RenderFrame frame, IRenderCamera camera);
  void OnEndPass(in RenderFrame frame, IRenderCamera camera);

  void OnRenderCamera(in RenderFrame frame, IRenderCamera camera);
}

/// <summary>
/// Base class for any <see cref="IRenderPass"/> implementation.
/// </summary>
public abstract class RenderPass : IRenderPass
{
  public virtual void OnPassAdded()
  {
  }

  public virtual void OnPassRemoved()
  {
  }

  public virtual void OnBeginFrame(in RenderFrame frame)
  {
  }

  public virtual void OnEndFrame(in RenderFrame frame)
  {
  }

  public virtual void OnBeginPass(in RenderFrame frame, IRenderCamera camera)
  {
  }

  public virtual void OnRenderCamera(in RenderFrame frame, IRenderCamera camera)
  {
  }

  public virtual void OnEndPass(in RenderFrame frame, IRenderCamera camera)
  {
  }

  public virtual void Dispose()
  {
  }
}

/// <summary>
/// A managed list of <see cref="IRenderPass"/>es.
/// </summary>
public sealed class RenderPassList : Collection<IRenderPass>
{
  protected override void OnItemAdded(IRenderPass item)
  {
    base.OnItemAdded(item);

    item.OnPassAdded();
  }

  protected override void OnItemRemoved(IRenderPass item)
  {
    item.OnPassRemoved();

    base.OnItemRemoved(item);
  }
}
