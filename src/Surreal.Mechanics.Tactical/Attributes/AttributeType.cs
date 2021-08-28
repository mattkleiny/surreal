using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Surreal.Mechanics.Tactical.Attributes
{
  [SuppressMessage("ReSharper", "UnusedTypeParameter")]
  public readonly record struct AttributeType<T>(string Name)
  {
    public override string ToString()
    {
      return Name;
    }

    public override int GetHashCode()
    {
      return string.GetHashCode(Name, StringComparison.OrdinalIgnoreCase);
    }

    public bool Equals(AttributeType<T> other)
    {
      return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
    }
  }

  public sealed class AttributeCollection
  {
    private readonly Dictionary<string, object> attributes = new(StringComparer.OrdinalIgnoreCase);

    public T Get<T>(AttributeType<T> attribute)
    {
      if (!attributes.TryGetValue(attribute.Name, out var entry))
      {
        return default!;
      }

      return ((Box<T>)entry).Value;
    }

    public void Set<T>(AttributeType<T> attribute, T value)
    {
      if (attributes.TryGetValue(attribute.Name, out var entry))
      {
        ((Box<T>)entry).Value = value;
      }
      else
      {
        attributes[attribute.Name] = new Box<T>(value);
      }
    }
  }
}
