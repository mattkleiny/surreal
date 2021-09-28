using System;
using System.Collections.Generic;

namespace Surreal.Mechanics.Tactical.Attributes
{
  public sealed class AttributeCollection
  {
    private readonly Dictionary<string, object> attributes = new(StringComparer.OrdinalIgnoreCase);

    public T Get<T>(AttributeType<T> attribute)
    {
      if (!attributes.TryGetValue(attribute.Name, out var entry))
      {
        return default!;
      }

      return (T) entry;
    }

    public void Set<T>(AttributeType<T> attribute, T value)
    {
      attributes[attribute.Name] = value!;
    }
  }
}
