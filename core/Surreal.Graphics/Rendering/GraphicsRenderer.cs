using Surreal.Assets;
using Surreal.Diagnostics.Logging;
using Surreal.Timing;

namespace Surreal.Graphics.Rendering;

/// <summary>Represents a viewport that can be used for rendering in an <see cref="GraphicsRenderer"/>.</summary>
public interface IRenderViewport
{
  Vector2 ScreenPosition      { get; }
  Vector2 ScreenDeltaPosition { get; }
}

/// <summary>Context for <see cref="GraphicsRenderer"/> operations.</summary>
public readonly record struct GraphicsContext(IGraphicsServer Server, IAssetManager Assets);

/// <summary>A renderer for graphics.</summary>
public abstract class GraphicsRenderer : IDisposable
{
  private static readonly ILog Log = LogFactory.GetLog<GraphicsRenderer>();

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
      Log.Error(exception, "An error occurred whilst initializing a renderer");
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
