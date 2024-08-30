using Surreal.Graphics.Rendering;
using Surreal.Timing;

namespace Surreal.Graphics;

/// <summary>
/// An event that indicates a frame should be rendered.
/// </summary>
public record struct RenderFrameEvent(DeltaTime DeltaTime, RenderFrame Frame)
{
  /// <summary>
  /// Attempts to get the render context of the given type for this frame.
  /// </summary>
  public readonly bool TryGetContext<TContext>(out TContext context)
    where TContext : RenderContext
  {
    return Frame.Contexts.TryGetContext(Frame, out context);
  }
}