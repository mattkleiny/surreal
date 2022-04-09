namespace Surreal.Effects;

/// <summary>Indicates an object has received a new <see cref="StatusEffect"/>.</summary>
public readonly record struct StatusEffectAdded(object Object, StatusEffect Effect);

/// <summary>Indicates an object has lost an existing <see cref="StatusEffect"/>.</summary>
public readonly record struct StatusEffectRemoved(object Object, StatusEffect Effect);
