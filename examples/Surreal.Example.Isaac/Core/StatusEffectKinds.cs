using Surreal.Effects;

namespace Isaac.Core;

/// <summary>Different kinds of <see cref="StatusEffect"/>s.</summary>
public static class StatusEffectKinds
{
  public static StatusEffectKind Frozen { get; } = new(nameof(Frozen), 1 << 0);
}
