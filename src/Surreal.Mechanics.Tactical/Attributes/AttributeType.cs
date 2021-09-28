using System;
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
}
