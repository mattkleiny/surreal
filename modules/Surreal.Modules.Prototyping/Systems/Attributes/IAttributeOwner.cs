﻿namespace Surreal.Systems.Attributes;

/// <summary>Represents an owner for <see cref="AttributeType"/>s</summary>
public interface IAttributeOwner
{
  /// <summary>Reads/writes base attribute values on the given object.</summary>
  int this[AttributeType attribute] { get; set; }
}