using Surreal.Timing;

namespace Surreal.Graphics.Rendering;

/// <summary>
/// Contextual data for a single rendered frame.
/// </summary>
public readonly record struct RenderFrame
{
  /// <summary>
  /// The delta time for this frame.
  /// </summary>
  public required DeltaTime DeltaTime { get; init; }  

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

  /// <summary>
  /// Attempts to get the render context of the given type for this frame.
  /// </summary>
  public readonly bool TryGetContext<TContext>(out TContext context)
    where TContext : RenderContext
  {
    return Contexts.TryGetContext(this, out context);
  }
}
