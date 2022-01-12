using System.Diagnostics.CodeAnalysis;
using Surreal.IO;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree.Expression;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree.Statement;

namespace Surreal.Graphics.Shaders;

/// <summary>Intrinsic outputs for shader instructions.</summary>
public enum IntrinsicType
{
  Position,
  Color,
}

/// <summary>Different types of precisions to use for primitive declarations.</summary>
public enum Precision
{
  Low,
  Medium,
  High,
}

/// <summary>Different kinds of <see cref="Primitive"/>s used in shaders.</summary>
public enum PrimitiveType
{
  Void,
  Bool,
  Int,
  UInt,
  Float,
  Matrix,
  Sampler,
}

/// <summary>Different archetypes of shader programs, for template expansions.</summary>
public enum ShaderArchetype
{
  Sprite,
}

/// <summary>Different types of shader programs.</summary>
public enum ShaderKind
{
  Vertex,
  Geometry,
  Fragment,
}

/// <summary>A primitive type declaration with optional cardinality for vector representations.</summary>
public readonly record struct Primitive(PrimitiveType Type, int? Cardinality = null, Precision? Precision = null);

/// <summary>Represents a parsed shader program, ready for interrogation and compilation.</summary>
public sealed record ShaderProgramDeclaration(string Path, ShaderArchetype Archetype, CompilationUnit CompilationUnit);

/// <summary>Common AST graph root for our shading languages.</summary>
public abstract record ShaderSyntaxTree
{
  /// <summary>
  /// A compilation unit for all shader instructions.
  /// <para/>
  /// This represents the top-level 'file' input to a shader and includes potentially many 'stages'
  /// </summary>
  public sealed record CompilationUnit : ShaderSyntaxTree
  {
    public CompilationUnit(params ShaderSyntaxTree[] nodes)
      : this(nodes.AsEnumerable())
    {
    }

    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public CompilationUnit(IEnumerable<ShaderSyntaxTree> nodes)
    {
      Uniforms  = nodes.OfType<UniformDeclaration>().ToImmutableArray();
      Varyings  = nodes.OfType<VaryingDeclaration>().ToImmutableArray();
      Constants = nodes.OfType<ConstantDeclaration>().ToImmutableArray();
      Includes  = nodes.OfType<Include>().ToImmutableArray();
      Functions = nodes.OfType<FunctionDeclaration>().ToImmutableArray();
      Stages    = nodes.OfType<StageDeclaration>().ToImmutableArray();
    }

    public ImmutableArray<UniformDeclaration>  Uniforms  { get; init; }
    public ImmutableArray<VaryingDeclaration>  Varyings  { get; init; }
    public ImmutableArray<ConstantDeclaration> Constants { get; init; }
    public ImmutableArray<Include>             Includes  { get; init; }
    public ImmutableArray<FunctionDeclaration> Functions { get; init; }
    public ImmutableArray<StageDeclaration>    Stages    { get; init; }
  }

  /// <summary>A single statement in a shader program.</summary>
  public abstract record Statement : ShaderSyntaxTree
  {
    /// <summary>A full-line comment</summary>
    /// <example>// A full-line comment</example>
    public sealed record Comment(string Text) : Statement;

    /// <summary>Includes a module of the given name from the given path.</summary>
    /// <example>#include "local://Assets/shaders/test.shader"</example>
    public sealed record Include(VirtualPath Path) : Statement;

    /// <summary>Declares a uniform parameter to the program.</summary>
    /// <example>uniform vec3 _direction;</example>
    public sealed record UniformDeclaration(Primitive Type, string Name) : Statement;

    /// <summary>Declares a varying parameter to the program.</summary>
    /// <example>varying vec3 _direction;</example>
    public sealed record VaryingDeclaration(Primitive Type, string Name) : Statement;

    /// <summary>Declares a constant primitive.</summary>
    /// <example>const vec3 test = vec3(1,1,1);</example>
    public sealed record ConstantDeclaration(Primitive Type, string Name, Expression Value) : Statement;

    /// <summary>Assigns a value to a variable.</summary>
    /// <example>test = vec3(1,1,1);</example>
    public sealed record Assignment(string Variable, Expression Value) : Statement;

    /// <summary>Assigns a value to an intrinsic.</summary>
    /// <example>COLOR = vec3(1,1,1);</example>
    public sealed record IntrinsicAssignment(IntrinsicType Intrinsic, Expression Value) : Statement;

    /// <summary>Declares a shader stage function.</summary>
    /// <example>void fragment() { ... }</example>
    public sealed record StageDeclaration(ShaderKind Kind) : Statement
    {
      public StageDeclaration(ShaderKind kind, params ShaderSyntaxTree[] nodes)
        : this(kind)
      {
        Parameters = nodes.OfType<Parameter>().ToImmutableArray();
        Statements = nodes.OfType<Statement>().ToImmutableArray();
      }

      public ImmutableArray<Parameter> Parameters { get; init; }
      public ImmutableArray<Statement> Statements { get; init; }
    }

    /// <summary>Declares a standard function.</summary>
    /// <example>float circle(float radius) { ... }</example>
    public sealed record FunctionDeclaration(Primitive ReturnType, string Name) : Statement
    {
      public FunctionDeclaration(Primitive returnType, string name, params ShaderSyntaxTree[] nodes)
        : this(returnType, name)
      {
        Parameters = nodes.OfType<Parameter>().ToImmutableArray();
        Statements = nodes.OfType<Statement>().ToImmutableArray();
      }

      public ImmutableArray<Parameter> Parameters { get; init; }
      public ImmutableArray<Statement> Statements { get; init; }
    }

    /// <summary>Standard control flow variants.</summary>
    public abstract record ControlFlow : Statement
    {
      /// <summary>Checks if a value is true or false.</summary>
      /// <example>if (test) { ... }</example>
      public sealed record If(Expression value, params ShaderSyntaxTree[] Body) : ControlFlow;
    }
  }

  /// <summary>A single expression, composite within a larger statement.</summary>
  public abstract record Expression : ShaderSyntaxTree
  {
    /// <summary>A constant value.</summary>
    /// <example>42</example>
    public sealed record Constant(object Value) : Expression;

    /// <summary>A list of values.</summary>
    /// <example>1, 2, 3</example>
    public sealed record Variadic(params Expression[] Values) : Expression;

    /// <summary>Declares a parameter.</summary>
    /// <example>int parameter1</example>
    public sealed record Parameter(Primitive Type, string Name) : Expression;

    /// <summary>A constructor expression for a primitive value.</summary>
    /// <example>vec3(1, 1, 1)</example>
    public sealed record Constructor(Primitive Type, Expression Value) : Expression;

    /// <summary>A simple binary operator expression.</summary>
    /// <example>1 + 2</example>
    public sealed record Binary(BinaryOperator Operator, Expression Left, Expression Right) : Expression;

    /// <summary>A simple unary operator expression.</summary>
    /// <example>-4</example>
    public sealed record Unary(UnaryOperator Operator, Expression Value) : Expression;
  }

  /// <summary>Binary operators used in binary expressions.</summary>
  public enum BinaryOperator
  {
    Add,
    Subtract,
    Multiply,
    Divide,
  }

  /// <summary>Unary operators used in unary expressions.</summary>
  public enum UnaryOperator
  {
    Negate,
  }
}
