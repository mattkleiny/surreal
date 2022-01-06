namespace Surreal.Storage;

/// <summary>Adds metadata to a component type.</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class ComponentAttribute : Attribute
{
  public Type? StorageType { get; set; }
}
