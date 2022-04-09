using JetBrains.Annotations;
using Surreal.Actions;

namespace Surreal.Utilities;

/// <summary>Associates a name with an <see cref="IAction"/> for easier discovery.</summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class ExportAttribute : Attribute
{
  public string? Name        { get; init; }
  public string? Description { get; init; }
  public string? Category    { get; init; }
}
