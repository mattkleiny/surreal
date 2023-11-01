using Surreal.IO;
using static Surreal.Graphics.Materials.ShaderSyntaxTree;
using static Surreal.Graphics.Materials.ShaderSyntaxTree.Statement;
using static Surreal.Graphics.Materials.ShaderSyntaxTree.Expression;

namespace Surreal.Graphics.Materials;

/// <summary>
/// Different types of precisions to use for primitive declarations.
/// </summary>
public enum ShaderPrecision
{
  Low,
  Medium,
  High,
}

/// <summary>
/// Different kinds of <see cref="ShaderPrimitive"/>s used in shaders.
/// </summary>
public enum ShaderPrimitiveType
{
  Void,
  Bool,
  Int,
  UInt,
  Float,
  Matrix,
  Sampler,
}

/// <summary>
/// Different types of shader programs.
/// </summary>
public enum ShaderKind
{
  Vertex,
  Fragment,
}

/// <summary>
/// A primitive type declaration with optional cardinality for vector representations.
/// </summary>
public readonly record struct ShaderPrimitive(ShaderPrimitiveType Type, int? Cardinality = null, ShaderPrecision? Precision = null);

/// <summary>
/// Represents a parsed shader program, ready for interrogation and compilation.
/// </summary>
public sealed record ShaderDeclaration(string Path, CompilationUnit CompilationUnit)
{
  // TODO: implement these sorts of queries
  public bool RequiresTransparentPass => throw new NotImplementedException();
  public bool RequiresGrabPass => throw new NotImplementedException();
}

/// <summary>
/// Common AST graph root for our shading languages.
/// </summary>
public abstract record ShaderSyntaxTree
{
  /// <summary>
  /// A compilation unit for all shader instructions.
  /// <para/>
  /// This represents the top-level 'file' input to a shader and includes potentially many 'stages'
  /// </summary>
  public sealed record CompilationUnit : ShaderSyntaxTree
  {
    public ShaderTypeDeclaration ShaderType { get; init; } = new("none");
    public ImmutableHashSet<Include> Includes { get; init; } = ImmutableHashSet<Include>.Empty;
    public ImmutableArray<UniformDeclaration> Uniforms { get; init; } = ImmutableArray<UniformDeclaration>.Empty;
    public ImmutableArray<VaryingDeclaration> Varyings { get; init; } = ImmutableArray<VaryingDeclaration>.Empty;
    public ImmutableArray<ConstantDeclaration> Constants { get; init; } = ImmutableArray<ConstantDeclaration>.Empty;
    public ImmutableArray<FunctionDeclaration> Functions { get; init; } = ImmutableArray<FunctionDeclaration>.Empty;
    public ImmutableArray<StageDeclaration> Stages { get; init; } = ImmutableArray<StageDeclaration>.Empty;

    public CompilationUnit MergeWith(CompilationUnit other) => this with
    {
      Includes = Includes.Union(other.Includes),
      Uniforms = Uniforms.AddRange(other.Uniforms),
      Varyings = Varyings.AddRange(other.Varyings),
      Constants = Constants.AddRange(other.Constants),
      Functions = Functions.AddRange(other.Functions),
      Stages = Stages.AddRange(other.Stages),
    };
  }

  /// <summary>
  /// A single statement in a shader program.
  /// </summary>
  public abstract record Statement : ShaderSyntaxTree
  {
    /// <summary>A full-line comment</summary>
    /// <example>// A full-line comment</example>
    public sealed record Comment(string Text) : Statement;

    /// <summary>Includes a module of the given name from the given path.</summary>
    /// <example>#include "local://Assets/shaders/test.shade"</example>
    public sealed record Include(VirtualPath Path) : Statement;

    /// <summary>Indicates the type of shader being compiled.</summary>
    /// <example>#shader_type sprite</example>
    public sealed record ShaderTypeDeclaration(string Type) : Statement;

    /// <summary>Declares a uniform parameter to the program.</summary>
    /// <example>uniform vec3 _direction;</example>
    public sealed record UniformDeclaration(ShaderPrimitive Type, string Name) : Statement;

    /// <summary>Declares a varying parameter to the program.</summary>
    /// <example>varying vec3 _direction;</example>
    public sealed record VaryingDeclaration(ShaderPrimitive Type, string Name) : Statement;

    /// <summary>Declares a constant primitive.</summary>
    /// <example>const vec3 test = vec3(1,1,1);</example>
    public sealed record ConstantDeclaration(ShaderPrimitive Type, string Name, Expression Value) : Statement;

    /// <summary>Declares a shader stage function.</summary>
    /// <example>void fragment() { ... }</example>
    public sealed record StageDeclaration(ShaderKind Kind) : Statement
    {
      public ImmutableArray<Parameter> Parameters { get; init; } = ImmutableArray<Parameter>.Empty;
      public ImmutableArray<Statement> Statements { get; init; } = ImmutableArray<Statement>.Empty;
    }

    /// <summary>Declares a standard function.</summary>
    /// <example>float circle(vec3 position, float radius) { ... }</example>
    public sealed record FunctionDeclaration(ShaderPrimitive ReturnType, string Name) : Statement
    {
      public ImmutableArray<Parameter> Parameters { get; init; } = ImmutableArray<Parameter>.Empty;
      public ImmutableArray<Statement> Statements { get; init; } = ImmutableArray<Statement>.Empty;
    }

    /// <summary>Assigns a value to a variable.</summary>
    /// <example>test = vec3(1,1,1);</example>
    public sealed record Assignment(string Variable, Expression Value) : Statement;

    /// <summary>Returns a value from the function.</summary>
    /// <example>return vec3(1,1,1);</example>
    public sealed record Return(Expression Value) : Statement;

    /// <summary>A statement that embodies a single expression.</summary>
    /// <example>test(3.14159f);</example>
    public sealed record StatementExpression(Expression Body) : Statement;

    /// <summary>Standard control flow variants.</summary>
    public abstract record ControlFlow : Statement
    {
      /// <summary>Checks if a value is true or false, with a single optional else clause.</summary>
      /// <example>if (test) { ... } else { ... }</example>
      public sealed record If(Expression Condition) : ControlFlow
      {
        public ImmutableArray<Statement> TrueBranch { get; init; } = ImmutableArray<Statement>.Empty;
        public ImmutableArray<Statement> FalseBranch { get; init; } = ImmutableArray<Statement>.Empty;
      }

      /// <summary>Evaluates the given statements in a standard 'while' loop</summary>
      /// <example>while (test) { ... }</example>
      public sealed record While(Expression Condition) : ControlFlow
      {
        public ImmutableArray<Statement> Statements { get; init; } = ImmutableArray<Statement>.Empty;
      }

      /// <summary>Evaluates the given statements in a standard 'for' loop</summary>
      /// <example>if (test) { ... } else { ... }</example>
      public sealed record For(Expression Condition) : ControlFlow
      {
        public ImmutableArray<Statement> Statements { get; init; } = ImmutableArray<Statement>.Empty;
      }
    }
  }

  /// <summary>
  /// A single expression, composite within a larger statement.
  /// </summary>
  public abstract record Expression : ShaderSyntaxTree
  {
    /// <summary>A constant value.</summary>
    /// <example>42</example>
    public sealed record Constant(object Value) : Expression;

    /// <summary>An identifier reference.</summary>
    /// <example>test_variable</example>
    public sealed record Symbol(string Name) : Expression;

    /// <summary>A list of values.</summary>
    /// <example>1, 2, 3</example>
    public sealed record Variadic(params Expression[] Values) : Expression;

    /// <summary>Declares a parameter.</summary>
    /// <example>int parameter1</example>
    public sealed record Parameter(ShaderPrimitive Type, string Name) : Expression;

    /// <summary>A constructor expression for a primitive value.</summary>
    /// <example>vec3(1, 1, 1)</example>
    public sealed record TypeConstructor(ShaderPrimitive Type, Expression Value) : Expression;

    /// <summary>A texture sampler operation expression.</summary>
    /// <example>SAMPLE(_Texture, input.uv)</example>
    public sealed record SampleOperation(string Name, Expression Value) : Expression;

    /// <summary>A simple binary operator expression.</summary>
    /// <example>1 + 2</example>
    public sealed record BinaryOperation(BinaryOperator Operator, Expression Left, Expression Right) : Expression;

    /// <summary>A simple unary operator expression.</summary>
    /// <example>-4</example>
    public sealed record UnaryOperation(UnaryOperator Operator, Expression Value) : Expression;
  }

  /// <summary>
  /// Binary operators used in binary expressions.
  /// </summary>
  public enum BinaryOperator
  {
    Add,
    Subtract,
    Multiply,
    Divide,

    Equal,
    NotEqual,
  }

  /// <summary>
  /// Unary operators used in unary expressions.
  /// </summary>
  public enum UnaryOperator
  {
    Negate,
  }
}
