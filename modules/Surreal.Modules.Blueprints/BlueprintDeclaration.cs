using Surreal.Assets;
using Surreal.IO;
using static Surreal.BlueprintSyntaxTree;

namespace Surreal;

/// <summary>Different kinds of <see cref="BlueprintArchetype"/>s.</summary>
public enum BlueprintArchetypeKind
{
  Item,
  Entity,
}

/// <summary>Represents a parsed blueprint, ready for interrogation and compilation.</summary>
public sealed record BlueprintDeclaration(string Path)
{
  public ImmutableArray<IncludeStatement>   Includes   { get; init; } = ImmutableArray<IncludeStatement>.Empty;
  public ImmutableArray<BlueprintArchetype> Archetypes { get; init; } = ImmutableArray<BlueprintArchetype>.Empty;

  public BlueprintDeclaration MergeWith(BlueprintDeclaration other) => this with
  {
    Includes = Includes.AddRange(other.Includes),
    Archetypes = Archetypes.AddRange(other.Archetypes)
  };
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="BlueprintDeclaration"/>s.</summary>
public sealed class BlueprintDeclarationLoader : AssetLoader<BlueprintDeclaration>
{
  private readonly BlueprintParser parser;

  public BlueprintDeclarationLoader()
    : this(new BlueprintParser())
  {
  }

  public BlueprintDeclarationLoader(BlueprintParser parser)
  {
    this.parser = parser;
  }

  public override async ValueTask<BlueprintDeclaration> LoadAsync(AssetLoaderContext context, ProgressToken progressToken = default)
  {
    return await parser.ParseAsync(context.Path, progressToken.CancellationToken);
  }
}

/// <summary>Common AST graph root for our blueprint languages.</summary>
public abstract record BlueprintSyntaxTree
{
  /// <summary>Describes a single archetype and it's attributes.</summary>
  public sealed record BlueprintArchetype(BlueprintArchetypeKind Kind, string Name) : BlueprintSyntaxTree
  {
    public ImmutableArray<string>               BaseTypes  { get; init; } = ImmutableArray<string>.Empty;
    public ImmutableArray<TagDeclaration>       Tags       { get; init; } = ImmutableArray<TagDeclaration>.Empty;
    public ImmutableArray<AttributeDeclaration> Attributes { get; init; } = ImmutableArray<AttributeDeclaration>.Empty;
    public ImmutableArray<ComponentDeclaration> Components { get; init; } = ImmutableArray<ComponentDeclaration>.Empty;
    public ImmutableArray<EventDeclaration>     Events     { get; init; } = ImmutableArray<EventDeclaration>.Empty;

    public override string ToString() => $"{Kind} {Name}";
  }

  /// <summary>Includes another module.</summary>
  /// <example>#include "Assets/test.blueprint"</example>
  public sealed record IncludeStatement(VirtualPath Path) : BlueprintSyntaxTree
  {
    public override string ToString() => $"#include '{Path}'";
  }

  /// <summary>Declares a tag in a blueprint.</summary>
  /// <example>#tag Usable</example>
  public sealed record TagDeclaration(string Name) : BlueprintSyntaxTree
  {
    public override string ToString() => $"Tag: {Name}";
  }

  /// <summary>Declares a single attribute in a blueprint.</summary>
  /// <example>attribute Damage(10);</example>
  public sealed record AttributeDeclaration(string Name) : BlueprintSyntaxTree
  {
    public bool                       IsOverride { get; init; } = false;
    public ImmutableArray<Expression> Parameters { get; init; } = ImmutableArray<Expression>.Empty;

    public override string ToString() => $"Attribute: {Name}";
  }

  /// <summary>Declares a single component in a blueprint.</summary>
  /// <example>component Health(100);</example>
  public sealed record ComponentDeclaration(string Name) : BlueprintSyntaxTree
  {
    public bool                       IsOverride { get; init; } = false;
    public ImmutableArray<Expression> Parameters { get; init; } = ImmutableArray<Expression>.Empty;

    public override string ToString() => $"Component: {Name}";
  }

  /// <summary>Declares a single event in a blueprint.</summary>
  /// <example>event DamageReceived(Health);</example>
  public sealed record EventDeclaration(string Name) : BlueprintSyntaxTree
  {
    public ImmutableArray<Expression> Parameters { get; init; } = ImmutableArray<Expression>.Empty;

    public override string ToString() => $"Event: {Name}";
  }

  /// <summary>Defines a basic expression that can be evaluated at runtime</summary>
  /// <example>1 + 1</example>
  public abstract record Expression : BlueprintSyntaxTree
  {
    /// <summary>An identifier name</summary>
    /// <example>PI</example>
    public sealed record Identifier(string Name) : Expression
    {
      public override string ToString() => Name;
    }

    /// <summary>A constant value</summary>
    /// <example>3.14159</example>
    public sealed record Constant(object Literal) : Expression
    {
      public override string? ToString() => Literal.ToString();
    }

    /// <summary>A unary operation</summary>
    /// <example>-42</example>
    public sealed record Unary(UnaryOperator Operator, Expression Left, Expression Right) : Expression;

    /// <summary>A binary operation</summary>
    /// <example>a + b</example>
    public sealed record Binary(BinaryOperator Operator, Expression Left, Expression Right) : Expression;
  }

  public enum UnaryOperator
  {
    Negate,
  }

  public enum BinaryOperator
  {
    Plus,
    Minus,
    Times,
    Divide,
  }
}
