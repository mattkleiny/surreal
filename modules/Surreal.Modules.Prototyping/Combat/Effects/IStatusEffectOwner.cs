namespace Surreal.Combat.Effects;

/// <summary>Indicates an object that may receive <see cref="StatusEffect"/>s.</summary>
public interface IStatusEffectOwner
{
  /// <summary>Retrieves the <see cref="StatusEffects"/>s of the object.</summary>
  StatusEffectCollection StatusEffects { get; }
}
