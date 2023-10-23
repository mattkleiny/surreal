using Surreal.Collections;

namespace Surreal.Graphics.Rendering;

/// <summary>
/// A single render pass in a <see cref="RenderPassList"/>.
/// </summary>
public interface IRenderPass : IDisposable
{
  /// <summary>
  /// True if the pass is enabled.
  /// </summary>
  bool IsEnabled { get; }

  void OnBeginFrame(in RenderFrame frame);
  void OnEndFrame(in RenderFrame frame);

  void OnBeginViewport(in RenderFrame frame, IRenderViewport viewport);
  void OnRenderViewport(in RenderFrame frame, IRenderViewport viewport);
  void OnEndViewport(in RenderFrame frame, IRenderViewport viewport);
}

/// <summary>
/// Base class for any <see cref="IRenderPass"/> implementation.
/// </summary>
public abstract class RenderPass : IRenderPass
{
  public virtual bool IsEnabled => true;

  public virtual void OnBeginFrame(in RenderFrame frame)
  {
  }

  public virtual void OnEndFrame(in RenderFrame frame)
  {
  }

  public virtual void OnBeginViewport(in RenderFrame frame, IRenderViewport viewport)
  {
  }

  public virtual void OnRenderViewport(in RenderFrame frame, IRenderViewport viewport)
  {
  }

  public virtual void OnEndViewport(in RenderFrame frame, IRenderViewport viewport)
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
  /// <summary>
  /// Inserts a render pass before a specific pass.
  /// </summary>
  public void InsertBefore<TOtherPass>(IRenderPass pass)
    where TOtherPass : IRenderPass
  {
    for (var i = 0; i < Count; i++)
    {
      var instance = this[i];
      if (instance is TOtherPass)
      {
        Insert(i, pass);
        return;
      }
    }

    Add(pass);
  }

  /// <summary>
  /// Inserts a render pass after a specific pass.
  /// </summary>
  public void InsertAfter<TOtherPass>(IRenderPass pass)
    where TOtherPass : IRenderPass
  {
    for (var i = 0; i < Count; i++)
    {
      var instance = this[i];
      if (instance is TOtherPass)
      {
        if (i == Count - 1)
        {
          Add(pass);
          return;
        }

        Insert(i + 1, pass);
        return;
      }
    }

    Add(pass);
  }
}
