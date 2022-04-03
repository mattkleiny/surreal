namespace Surreal.Attributes;

/// <summary>Indicates a type of attribute.</summary>
public readonly record struct AttributeType<T>(string Name)
{
  public override string ToString()
  {
    return Name;
  }
}
