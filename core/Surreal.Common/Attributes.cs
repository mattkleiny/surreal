using JetBrains.Annotations;

namespace Surreal;

/// <summary>
/// Associates metadata with a type for use in editors/etc.
/// </summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class EditorDescriptionAttribute : Attribute
{
  public string? Name { get; init; }
  public string? Description { get; init; }
  public string? Category { get; init; }
}

/// <summary>
/// Indicates the associated type is made more visible for testing purposes, only.
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public sealed class VisibleForTestingAttribute : Attribute
{
}
