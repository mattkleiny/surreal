namespace Surreal.Attributes;

/// <summary>Indicates an object has had it's attributes changed.</summary>
public readonly record struct AttributeChanged(object Object, AttributeType Attribute, int Amount);
