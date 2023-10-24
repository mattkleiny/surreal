using Surreal.Diagnostics.Profiling;
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
  void Render(IRenderScene scene, DeltaTime deltaTime);
}

/// <summary>
/// Convenience base class for <see cref="IRenderPipeline"/> implementations.
/// </summary>
public abstract class RenderPipeline(IGraphicsBackend backend) : IRenderPipeline
{
  private readonly TimeStamp _startTime = TimeStamp.Now;

  /// <summary>
  /// The <see cref="RenderContextManager"/> for the pipeline.
  /// </summary>
  public RenderContextManager Contexts { get; } = new();

  /// <inheritdoc/>
  public void Render(IRenderScene scene, DeltaTime deltaTime)
  {
    var frame = new RenderFrame
    {
      DeltaTime = deltaTime,
      TotalTime = TimeStamp.Now - _startTime,
      Backend = backend,
      Contexts = Contexts,
      Scene = scene,
      Viewport = backend.GetViewportSize()
    };

    OnBeginFrame(in frame);

    foreach (var camera in scene.CullActiveViewports())
    {
      RenderViewport(in frame, camera);
    }

    OnEndFrame(in frame);
  }

  private void RenderViewport(in RenderFrame frame, IRenderViewport viewport)
  {
    Contexts.OnBeginViewport(in frame, viewport);

    OnRenderViewport(in frame, viewport);

    Contexts.OnEndViewport(in frame, viewport);
  }

  protected virtual void OnBeginFrame(in RenderFrame frame)
  {
    Contexts.OnBeginFrame(in frame);
  }

  protected virtual void OnRenderViewport(in RenderFrame frame, IRenderViewport viewport)
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
  private static readonly IProfiler Profiler = ProfilerFactory.GetProfiler<MultiPassRenderPipeline>();

  /// <summary>
  /// The list of render passes in the pipeline.
  /// </summary>
  public RenderPassList Passes { get; } = new();

  protected override void OnBeginFrame(in RenderFrame frame)
  {
    base.OnBeginFrame(in frame);

    foreach (var pass in Passes)
    {
      if (pass.IsEnabled)
      {
        pass.OnBeginFrame(in frame);
      }
    }
  }

  protected override void OnEndFrame(in RenderFrame frame)
  {
    base.OnEndFrame(in frame);

    foreach (var pass in Passes)
    {
      if (pass.IsEnabled)
      {
        pass.OnEndFrame(in frame);
      }
    }
  }

  protected override void OnRenderViewport(in RenderFrame frame, IRenderViewport viewport)
  {
    base.OnRenderViewport(in frame, viewport);

    foreach (var pass in Passes)
    {
      if (pass.IsEnabled)
      {
        using (Profiler.Track(pass.Name))
        {
          pass.OnBeginViewport(in frame, viewport);
        }
      }
    }

    foreach (var pass in Passes)
    {
      if (pass.IsEnabled)
      {
        using (Profiler.Track(pass.Name))
        using (new GraphicsDebugScope(frame.Backend, pass.Name))
        {
          Contexts.OnBeginPass(in frame, viewport);

          pass.OnBeginPass(in frame, viewport);
          pass.OnExecutePass(in frame, viewport);
          pass.OnEndPass(in frame, viewport);

          Contexts.OnEndPass(in frame, viewport);
        }
      }
    }

    foreach (var pass in Passes)
    {
      if (pass.IsEnabled)
      {
        using (Profiler.Track(pass.Name))
        {
          pass.OnEndViewport(in frame, viewport);
        }
      }
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
