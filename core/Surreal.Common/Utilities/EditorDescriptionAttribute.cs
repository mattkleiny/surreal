using JetBrains.Annotations;

namespace Surreal.Utilities;

/// <summary>Associates metadata with a type for use in editors/etc.</summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class EditorDescriptionAttribute : Attribute
{
  public string? Name        { get; init; }
  public string? Description { get; init; }
  public string? Category    { get; init; }
}
