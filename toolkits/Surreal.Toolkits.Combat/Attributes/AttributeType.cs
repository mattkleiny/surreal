using Surreal.Collections;

namespace Surreal.Attributes;

/// <summary>Indicates a type of attribute.</summary>
public readonly record struct AttributeType(string Name)
{
  public AttributeType(Property<int> property)
    : this(property.Key)
  {
    Property = property;
  }

  public Property<int> Property { get; } = new(Name);
}
