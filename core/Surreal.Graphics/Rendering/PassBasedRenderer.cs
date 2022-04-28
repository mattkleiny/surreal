using Surreal.Collections;
using Surreal.Diagnostics.Logging;
using Surreal.Timing;

namespace Surreal.Graphics.Rendering;

/// <summary>A <see cref="GraphicsRenderer"/> based on <see cref="RenderPass"/>es.</summary>
public abstract class PassBasedRenderer : GraphicsRenderer
{
  public RenderPassList Passes { get; } = new();

  public override void BeginFrame(GraphicsContext context, TimeDelta deltaTime)
  {
    base.BeginFrame(context, deltaTime);

    foreach (var pass in Passes)
    {
      pass.BeginFrame(context, deltaTime);
    }
  }

  public override void BeginCamera(GraphicsContext context, TimeDelta deltaTime, IRenderViewport viewport)
  {
    base.BeginCamera(context, deltaTime, viewport);

    foreach (var pass in Passes)
    {
      pass.BeginCamera(context, deltaTime, viewport);
    }
  }

  public override void EndCamera(GraphicsContext context, TimeDelta deltaTime, IRenderViewport viewport)
  {
    base.EndCamera(context, deltaTime, viewport);

    foreach (var pass in Passes)
    {
      pass.EndCamera(context, deltaTime, viewport);
    }
  }

  public override void EndFrame(GraphicsContext context, TimeDelta deltaTime)
  {
    base.EndFrame(context, deltaTime);

    foreach (var pass in Passes)
    {
      pass.EndFrame(context, deltaTime);
    }
  }
}

/// <summary>A single pass in a <see cref="PassBasedRenderer"/>.</summary>
public abstract class RenderPass : IDisposable
{
  private static readonly ILog Log = LogFactory.GetLog<RenderPass>();

  public bool IsInitialized { get; private set; }
  public bool IsDisposed    { get; private set; }

  public virtual async void Initialize(GraphicsContext context)
  {
    try
    {
      await InitializeAsync(context);
    }
    catch (Exception exception)
    {
      Log.Error(exception, "An error occurred whilst initializing a render pass");
    }
  }

  protected virtual Task InitializeAsync(GraphicsContext context)
  {
    return Task.CompletedTask;
  }

  public virtual void BeginFrame(GraphicsContext context, TimeDelta deltaTime)
  {
    if (!IsInitialized)
    {
      Initialize(context);

      IsInitialized = true;
    }
  }

  public virtual void BeginCamera(GraphicsContext context, TimeDelta deltaTime, IRenderViewport viewport)
  {
  }

  public virtual void EndCamera(GraphicsContext context, TimeDelta deltaTime, IRenderViewport viewport)
  {
  }

  public virtual void EndFrame(GraphicsContext context, TimeDelta deltaTime)
  {
  }

  public void Dispose()
  {
    if (!IsDisposed)
    {
      Dispose(true);

      IsDisposed = true;
    }
  }

  protected virtual void Dispose(bool managed)
  {
  }
}

/// <summary>A sorted list of <see cref="RenderPass"/>es.</summary>
public sealed class RenderPassList : ListDecorator<RenderPass>
{
  public void TogglePass<TPass>(bool value)
    where TPass : RenderPass, new()
  {
    var alreadyExists = Contains<TPass>();

    if (!alreadyExists && value)
    {
      Add(new TPass());
    }
    else if (alreadyExists && !value)
    {
      Remove<TPass>();
    }
  }

  public bool Contains<TPass>()
    where TPass : RenderPass
  {
    foreach (var item in Items)
    {
      if (item is TPass)
      {
        return true;
      }
    }

    return false;
  }

  public bool Remove<TPass>()
    where TPass : RenderPass
  {
    foreach (var item in Items)
    {
      if (item is TPass)
      {
        return base.Remove(item);
      }
    }

    return false;
  }

  protected override void OnItemAdded(RenderPass item)
  {
    base.OnItemAdded(item);

    Items.Sort(SortRenderPasses);
  }

  protected override void OnItemRemoved(RenderPass item)
  {
    base.OnItemRemoved(item);

    Items.Sort(SortRenderPasses);
  }

  private static int SortRenderPasses(RenderPass x, RenderPass y)
  {
    // TODO: implement me
    return -1;
  }
}
