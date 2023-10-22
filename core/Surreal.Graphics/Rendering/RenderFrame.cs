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
  /// The total time since rendering started.
  /// </summary>
  public required DeltaTime TotalTime { get; init; }

  /// <summary>
  /// The <see cref="IGraphicsBackend"/> for this frame.
  /// </summary>
  public required IGraphicsBackend Backend { get; init; }

  /// <summary>
  /// The <see cref="IRenderContextManager"/> for this frame.
  /// </summary>
  public required IRenderContextManager Contexts { get; init; }

  /// <summary>
  /// The <see cref="IRenderScene"/> being rendered.
  /// </summary>
  public required IRenderScene Scene { get; init; }

  /// <summary>
  /// The viewport size of the frame.
  /// </summary>
  public required Viewport Viewport { get; init; }
}
