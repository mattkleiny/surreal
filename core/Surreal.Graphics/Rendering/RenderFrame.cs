namespace Surreal.Graphics.Rendering;

/// <summary>
/// Contextual data for a single rendered frame.
/// </summary>
public readonly record struct RenderFrame
{
  /// <summary>
  /// The <see cref="IGraphicsBackend"/> for this frame.
  /// </summary>
  public required IGraphicsDevice Device { get; init; }

  /// <summary>
  /// The <see cref="IRenderContextManager"/> for this frame.
  /// </summary>
  public required IRenderContextManager Contexts { get; init; }

  /// <summary>
  /// The viewport size of the frame.
  /// </summary>
  public required Viewport Viewport { get; init; }
}
