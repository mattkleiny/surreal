using Surreal.Timing;

namespace Surreal.Graphics.Rendering;

/// <summary>
/// Indicates the associated type is a <see cref="IRenderPipeline"/> and names it.
/// </summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class)]
public sealed class RenderPipelineAttribute(string name) : Attribute
{
  public string Name { get; } = name;
}

/// <summary>
/// Represents a pipeline for graphics rendering.
/// </summary>
public interface IRenderPipeline : IDisposable
{
  /// <summary>
  /// Renders all the given cameras in the given scene.
  /// </summary>
  void Render(IRenderScene scene);
}

/// <summary>
/// Convenience base class for <see cref="IRenderPipeline"/> implementations.
/// </summary>
public abstract class RenderPipeline(IGraphicsBackend backend) : IRenderPipeline
{
  private readonly DeltaTimeClock _clock = new();
  private readonly TimeStamp _startTime = TimeStamp.Now;

  /// <summary>
  /// The <see cref="RenderContextManager"/> for the pipeline.
  /// </summary>
  public RenderContextManager Contexts { get; } = new(backend);

  /// <inheritdoc/>
  public void Render(IRenderScene scene)
  {
    var frame = new RenderFrame
    {
      DeltaTime = _clock.Tick(),
      TotalTime = TimeStamp.Now - _startTime,
      Backend = backend,
      Contexts = Contexts,
      Scene = scene,
      Viewport = backend.GetViewportSize()
    };

    OnBeginFrame(in frame);

    foreach (var camera in scene.CullVisibleCameras())
    {
      RenderCamera(in frame, camera);
    }

    OnEndFrame(in frame);
  }

  private void RenderCamera(in RenderFrame frame, IRenderCamera camera)
  {
    OnBeginCamera(in frame, camera);
    OnRenderCamera(in frame, camera);
    OnEndCamera(in frame, camera);
  }

  protected virtual void OnBeginFrame(in RenderFrame frame)
  {
    Contexts.OnBeginFrame(in frame);
  }

  protected virtual void OnBeginCamera(in RenderFrame frame, IRenderCamera camera)
  {
    Contexts.OnBeginCamera(in frame, camera);
  }

  protected virtual void OnRenderCamera(in RenderFrame frame, IRenderCamera camera)
  {
  }

  protected virtual void OnEndCamera(in RenderFrame frame, IRenderCamera camera)
  {
    Contexts.OnEndCamera(in frame, camera);
  }

  protected virtual void OnEndFrame(in RenderFrame frame)
  {
    Contexts.OnEndFrame(in frame);
  }

  public virtual void Dispose()
  {
    Contexts.Dispose();
  }
}

/// <summary>
/// A basic <see cref="RenderPipeline"/> that supports multiple render passes.
/// </summary>
public abstract class MultiPassRenderPipeline(IGraphicsBackend backend) : RenderPipeline(backend)
{
  /// <summary>
  /// The list of render passes in the pipeline.
  /// </summary>
  public RenderPassList Passes { get; } = new();

  protected override void OnBeginFrame(in RenderFrame frame)
  {
    base.OnBeginFrame(in frame);

    foreach (var pass in Passes)
    {
      pass.OnBeginFrame(in frame);
    }
  }

  protected override void OnBeginCamera(in RenderFrame frame, IRenderCamera camera)
  {
    base.OnBeginCamera(in frame, camera);

    foreach (var pass in Passes)
    {
      pass.OnBeginCamera(in frame, camera);
    }
  }

  protected override void OnRenderCamera(in RenderFrame frame, IRenderCamera camera)
  {
    base.OnRenderCamera(in frame, camera);

    foreach (var pass in Passes)
    {
      pass.OnRenderCamera(in frame, camera);
    }
  }

  protected override void OnEndCamera(in RenderFrame frame, IRenderCamera camera)
  {
    base.OnEndCamera(in frame, camera);

    foreach (var pass in Passes)
    {
      pass.OnEndCamera(in frame, camera);
    }
  }

  protected override void OnEndFrame(in RenderFrame frame)
  {
    base.OnEndFrame(in frame);

    foreach (var pass in Passes)
    {
      pass.OnEndFrame(in frame);
    }
  }

  public override void Dispose()
  {
    for (var i = Passes.Count - 1; i >= 0; i--)
    {
      var pass = Passes[i];
      Passes.RemoveAt(i);

      pass.Dispose();
    }

    base.Dispose();
  }
}
