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
  /// The <see cref="IRenderContextManager"/> for this frame.
  /// </summary>
  public required IRenderContextManager Manager { get; init; }
}
