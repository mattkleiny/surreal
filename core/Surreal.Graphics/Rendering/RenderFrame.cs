using Surreal.Collections;
using Surreal.Timing;

namespace Surreal.Graphics.Rendering;

/// <summary>
/// Contextual data for a single rendered frame.
/// </summary>
public readonly record struct RenderFrame
{
  /// <summary>
  /// The time since the last frame.
  /// </summary>
  public required DeltaTime DeltaTime { get; init; }

  /// <summary>
  /// The <see cref="IGraphicsBackend"/> for this frame.
  /// </summary>
  public required IGraphicsBackend Backend { get; init; }

  /// <summary>
  /// The <see cref="IRenderContextManager"/> for this frame.
  /// </summary>
  public required IRenderContextManager Manager { get; init; }

  /// <summary>
  /// The <see cref="IRenderScene"/> being rendered.
  /// </summary>
  public required IRenderScene Scene { get; init; }

  /// <summary>
  /// The current camera being rendered.
  /// </summary>
  public IRenderCamera? Camera { get; init; }

  /// <summary>
  /// The objects that were culled by the camera.
  /// </summary>
  public ReadOnlySlice<IRenderObject> VisibleObjects { get; init; }
}
