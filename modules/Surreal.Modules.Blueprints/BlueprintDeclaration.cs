using static Surreal.BlueprintSyntaxTree;

namespace Surreal;

/// <summary>Represents a parsed blueprint, ready for interrogation and compilation.</summary>
public sealed record BlueprintDeclaration(string Path)
{
  public ImmutableArray<BlueprintArchetype> Archetypes { get; init; } = ImmutableArray<BlueprintArchetype>.Empty;
}

/// <summary>Common AST graph root for our blueprint languages.</summary>
public abstract record BlueprintSyntaxTree
{
  /// <summary>Describes a single archetype and it's attributes.</summary>
  public sealed record BlueprintArchetype(string Name) : BlueprintSyntaxTree
  {
    public ImmutableArray<AttributeDeclaration> Attributes { get; init; } = ImmutableArray<AttributeDeclaration>.Empty;
    public ImmutableArray<ComponentDeclaration> Components { get; init; } = ImmutableArray<ComponentDeclaration>.Empty;
    public ImmutableArray<EventDeclaration>     Events     { get; init; } = ImmutableArray<EventDeclaration>.Empty;
  }

  /// <summary>Declares a tag in a blueprint.</summary>
  /// <example>#tag Usable</example>
  public sealed record TagDeclaration(string Name) : BlueprintSyntaxTree;

  /// <summary>Declares a single attribute in a blueprint.</summary>
  /// <example>attribute Damage(10);</example>
  public sealed record AttributeDeclaration(string Name) : BlueprintSyntaxTree;

  /// <summary>Declares a single component in a blueprint.</summary>
  /// <example>component Health(100);</example>
  public sealed record ComponentDeclaration(string Name) : BlueprintSyntaxTree;

  /// <summary>Declares a single event in a blueprint.</summary>
  /// <example>event DamageReceived(Health);</example>
  public sealed record EventDeclaration(string Name) : BlueprintSyntaxTree;
}
