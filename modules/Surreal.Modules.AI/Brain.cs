using Surreal.Timing;

namespace Surreal;

/// <summary>Context for <see cref="Brain"/> operations.</summary>
public readonly record struct BrainContext(DeltaTime DeltaTime, LevelOfDetail LevelOfDetail, Priority Priority);

/// <summary>A brain for an intelligent actor</summary>
public abstract class Brain
{
  /// <summary>Advances the brain a single step.</summary>
  public abstract void Think(BrainContext context);
}
