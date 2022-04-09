namespace Surreal.Combat;

/// <summary>Indicates an object has received <see cref="Damage"/>.</summary>
public readonly record struct ObjectDamaged(object Object, Damage Damage);

/// <summary>Indicates an object has been destroyed.</summary>
public readonly record struct ObjectDestroyed(object Object);
