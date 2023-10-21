using Surreal.Collections;
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
  /// Initializes the pipeline.
  /// </summary>
  void Initialize();

  /// <summary>
  /// Renders all the given cameras in the given frame.
  /// </summary>
  void Render(ReadOnlySlice<IRenderCamera> cameras);
}

/// <summary>
/// Convenience base class for <see cref="IRenderPipeline"/> implementations.
/// </summary>
public abstract class RenderPipeline : IRenderPipeline
{
  private readonly DeltaTimeClock _clock = new();

  public void Render(ReadOnlySlice<IRenderCamera> cameras)
  {
    var frame = new RenderFrame
    {
      DeltaTime = _clock.Tick()
    };

    OnBeginFrame(in frame);

    foreach (var camera in cameras)
    {
      OnRenderCamera(in frame, camera);
    }

    OnEndFrame(in frame);
  }

  public virtual void Initialize()
  {
  }

  protected virtual void OnBeginFrame(in RenderFrame frame)
  {
  }

  protected virtual void OnEndFrame(in RenderFrame frame)
  {
  }

  protected virtual void OnRenderCamera(in RenderFrame frame, IRenderCamera camera)
  {
  }

  public virtual void Dispose()
  {
  }
}

/// <summary>
/// A basic <see cref="RenderPipeline"/> that supports multiple render passes.
/// </summary>
public abstract class MultiPassRenderPipeline : RenderPipeline
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

  protected override void OnEndFrame(in RenderFrame frame)
  {
    base.OnEndFrame(in frame);

    foreach (var pass in Passes)
    {
      pass.OnEndFrame(in frame);
    }
  }

  protected override void OnRenderCamera(in RenderFrame frame, IRenderCamera camera)
  {
    base.OnRenderCamera(in frame, camera);

    foreach (var pass in Passes)
    {
      pass.OnBeginPass(in frame, camera);
      pass.OnRenderCamera(in frame, camera);
      pass.OnEndPass(in frame, camera);
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
