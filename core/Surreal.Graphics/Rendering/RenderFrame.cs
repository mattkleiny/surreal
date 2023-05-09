using Surreal.Timing;

namespace Surreal.Graphics.Rendering;

/// <summary>
/// Contextual data for a single rendered frame.
/// </summary>
public readonly record struct RenderFrame
{
  public required TimeDelta DeltaTime { get; init; }
}
