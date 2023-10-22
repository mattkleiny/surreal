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
      Backend = backend,
      Contexts = Contexts,
      Scene = scene
    };

    OnBeginFrame(in frame);

    foreach (var camera in scene.CullVisibleCameras())
    {
      OnExecuteFrame(frame with
      {
        Camera = camera,
        VisibleObjects = camera.CullVisibleObjects()
      });
    }

    OnEndFrame(in frame);
  }

  protected virtual void OnBeginFrame(in RenderFrame frame)
  {
    Contexts.OnBeginFrame(in frame);
  }

  protected virtual void OnExecuteFrame(in RenderFrame frame)
  {
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

  protected override void OnExecuteFrame(in RenderFrame frame)
  {
    base.OnExecuteFrame(in frame);

    foreach (var pass in Passes)
    {
      pass.OnBeginPass(in frame);
      pass.OnExecutePass(in frame);
      pass.OnEndPass(in frame);
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
