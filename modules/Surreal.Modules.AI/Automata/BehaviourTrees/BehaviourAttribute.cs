using JetBrains.Annotations;

namespace Surreal.Automata.BehaviourTrees;

/// <summary>Indicates the associated type is used in a behaviour tree.</summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class)]
public class BehaviourAttribute : Attribute
{
  public string Name        { get; init; } = string.Empty;
  public string Description { get; init; } = string.Empty;
}
