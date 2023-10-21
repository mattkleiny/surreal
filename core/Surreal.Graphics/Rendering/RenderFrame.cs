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
}
