using Surreal.Assets;
using Surreal.IO;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree.Statement;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree.Expression;

namespace Surreal.Graphics.Shaders;

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
public sealed record ShaderDeclaration(string Path, ShaderCompilationUnit CompilationUnit);

/// <summary>An <see cref="AssetLoader{T}"/> for <see cref="ShaderDeclaration"/>s.</summary>
public sealed class ShaderDeclarationLoader : AssetLoader<ShaderDeclaration>
{
  private readonly ShaderParser parser;
  private readonly ImmutableHashSet<string> extensions;
  private readonly Encoding encoding;

  public ShaderDeclarationLoader(ShaderParser parser, params string[] extensions)
    : this(parser, extensions.AsEnumerable())
  {
  }

  public ShaderDeclarationLoader(ShaderParser parser, IEnumerable<string> extensions)
    : this(parser, extensions, Encoding.UTF8)
  {
  }

  public ShaderDeclarationLoader(ShaderParser parser, IEnumerable<string> extensions, Encoding encoding)
  {
    this.parser = parser;
    this.extensions = extensions.ToImmutableHashSet();
    this.encoding = encoding;
  }

  public override bool CanHandle(AssetLoaderContext context)
  {
    return base.CanHandle(context) && extensions.Contains(context.Path.Extension);
  }

  public override async ValueTask<ShaderDeclaration> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken)
  {
    return await parser.ParseAsync(context.Path, encoding, cancellationToken);
  }
}

/// <summary>
/// A compilation unit for all shader instructions.
/// <para/>
/// This represents the top-level 'file' input to a shader and includes potentially many 'stages'
/// </summary>
public sealed record ShaderCompilationUnit : ShaderSyntaxTree
{
  public ShaderTypeDeclaration               ShaderType { get; init; } = new("none");
  public ImmutableHashSet<Include>           Includes   { get; init; } = ImmutableHashSet<Include>.Empty;
  public ImmutableArray<UniformDeclaration>  Uniforms   { get; init; } = ImmutableArray<UniformDeclaration>.Empty;
  public ImmutableArray<VaryingDeclaration>  Varyings   { get; init; } = ImmutableArray<VaryingDeclaration>.Empty;
  public ImmutableArray<ConstantDeclaration> Constants  { get; init; } = ImmutableArray<ConstantDeclaration>.Empty;
  public ImmutableArray<FunctionDeclaration> Functions  { get; init; } = ImmutableArray<FunctionDeclaration>.Empty;
  public ImmutableArray<StageDeclaration>    Stages     { get; init; } = ImmutableArray<StageDeclaration>.Empty;

  public ShaderCompilationUnit MergeWith(ShaderCompilationUnit other) => this with
  {
    Includes = Includes.Union(other.Includes),
    Uniforms = Uniforms.AddRange(other.Uniforms),
    Varyings = Varyings.AddRange(other.Varyings),
    Constants = Constants.AddRange(other.Constants),
    Functions = Functions.AddRange(other.Functions),
    Stages = Stages.AddRange(other.Stages),
  };

  public override void Accept(ShaderVisitor visitor) => visitor.Visit(this);
}

/// <summary>Common AST graph root for our shading languages.</summary>
public abstract record ShaderSyntaxTree
{
  /// <summary>Visitation pattern for <see cref="ShaderSyntaxTree"/> nodes.</summary>
  public abstract void Accept(ShaderVisitor visitor);

  /// <summary>A single statement in a shader program.</summary>
  public abstract record Statement : ShaderSyntaxTree
  {
    /// <summary>A full-line comment</summary>
    /// <example>// A full-line comment</example>
    public sealed record Comment(string Text) : Statement
    {
      public override void Accept(ShaderVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>Includes a module of the given name from the given path.</summary>
    /// <example>#include "local://Assets/shaders/test.shade"</example>
    public sealed record Include(VirtualPath Path) : Statement
    {
      public override void Accept(ShaderVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>Indicates the type of shader being compiled.</summary>
    /// <example>#shader_type sprite</example>
    public sealed record ShaderTypeDeclaration(string Type) : Statement
    {
      public override void Accept(ShaderVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>Declares a uniform parameter to the program.</summary>
    /// <example>uniform vec3 _direction;</example>
    public sealed record UniformDeclaration(Primitive Type, string Name) : Statement
    {
      public override void Accept(ShaderVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>Declares a varying parameter to the program.</summary>
    /// <example>varying vec3 _direction;</example>
    public sealed record VaryingDeclaration(Primitive Type, string Name) : Statement
    {
      public override void Accept(ShaderVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>Declares a constant primitive.</summary>
    /// <example>const vec3 test = vec3(1,1,1);</example>
    public sealed record ConstantDeclaration(Primitive Type, string Name, Expression Value) : Statement
    {
      public override void Accept(ShaderVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>Declares a variable primitive.</summary>
    /// <example>vec3 test = vec3(1,1,1);</example>
    public sealed record VariableDeclaration(Primitive Type, string Name, Expression Value) : Statement
    {
      public override void Accept(ShaderVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>Declares a shader stage function.</summary>
    /// <example>void fragment() { ... }</example>
    public sealed record StageDeclaration(ShaderKind Kind) : Statement
    {
      public ImmutableArray<Parameter> Parameters { get; init; } = ImmutableArray<Parameter>.Empty;
      public ImmutableArray<Statement> Statements { get; init; } = ImmutableArray<Statement>.Empty;

      public override void Accept(ShaderVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>Declares a standard function.</summary>
    /// <example>float circle(vec3 position, float radius) { ... }</example>
    public sealed record FunctionDeclaration(Primitive ReturnType, string Name) : Statement
    {
      public ImmutableArray<Parameter> Parameters { get; init; } = ImmutableArray<Parameter>.Empty;
      public ImmutableArray<Statement> Statements { get; init; } = ImmutableArray<Statement>.Empty;

      public override void Accept(ShaderVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>Assigns a value to a variable.</summary>
    /// <example>test = vec3(1,1,1);</example>
    public sealed record Assignment(string Variable, Expression Value) : Statement
    {
      public override void Accept(ShaderVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>Returns a value from the function.</summary>
    /// <example>return vec3(1,1,1);</example>
    public sealed record Return(Expression Value) : Statement
    {
      public override void Accept(ShaderVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>A statement that embodies a single expression.</summary>
    /// <example>test(3.14159f);</example>
    public sealed record StatementExpression(Expression Body) : Statement
    {
      public override void Accept(ShaderVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>Standard control flow variants.</summary>
    public abstract record ControlFlow : Statement
    {
      /// <summary>Checks if a value is true or false, with a single optional else clause.</summary>
      /// <example>if (test) { ... } else { ... }</example>
      public sealed record If(Expression Condition) : ControlFlow
      {
        public ImmutableArray<Statement> TrueBranch  { get; init; } = ImmutableArray<Statement>.Empty;
        public ImmutableArray<Statement> FalseBranch { get; init; } = ImmutableArray<Statement>.Empty;

        public override void Accept(ShaderVisitor visitor) => visitor.Visit(this);
      }

      /// <summary>Evaluates the given statements in a standard 'while' loop</summary>
      /// <example>while (test) { ... }</example>
      public sealed record WhileLoop(Expression Condition) : ControlFlow
      {
        public ImmutableArray<Statement> Statements { get; init; } = ImmutableArray<Statement>.Empty;

        public override void Accept(ShaderVisitor visitor) => visitor.Visit(this);
      }

      /// <summary>Evaluates the given statements in a standard 'for' loop</summary>
      /// <example>if (test) { ... } else { ... }</example>
      public sealed record ForLoop(Expression Condition) : ControlFlow
      {
        public ImmutableArray<Statement> Statements { get; init; } = ImmutableArray<Statement>.Empty;

        public override void Accept(ShaderVisitor visitor) => visitor.Visit(this);
      }
    }
  }

  /// <summary>A single expression, composite within a larger statement.</summary>
  public abstract record Expression : ShaderSyntaxTree
  {
    /// <summary>A constant value.</summary>
    /// <example>42</example>
    public sealed record Constant(object Value) : Expression
    {
      public override void Accept(ShaderVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>An identifier reference.</summary>
    /// <example>test_variable</example>
    public sealed record Symbol(string Name) : Expression
    {
      public override void Accept(ShaderVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>A list of values.</summary>
    /// <example>1, 2, 3</example>
    public sealed record Variadic(params Expression[] Values) : Expression
    {
      public override void Accept(ShaderVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>Declares a parameter.</summary>
    /// <example>int parameter1</example>
    public sealed record Parameter(Primitive Type, string Name) : Expression
    {
      public override void Accept(ShaderVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>A constructor expression for a primitive value.</summary>
    /// <example>vec3(1, 1, 1)</example>
    public sealed record TypeConstructor(Primitive Type, Expression Value) : Expression
    {
      public override void Accept(ShaderVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>A texture sampler operation expression.</summary>
    /// <example>SAMPLE(_Texture, input.uv)</example>
    public sealed record SampleOperation(string Name, Expression Value) : Expression
    {
      public override void Accept(ShaderVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>A simple binary operator expression.</summary>
    /// <example>1 + 2</example>
    public sealed record BinaryOperation(BinaryOperator Operator, Expression Left, Expression Right) : Expression
    {
      public override void Accept(ShaderVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>A simple unary operator expression.</summary>
    /// <example>-4</example>
    public sealed record UnaryOperation(UnaryOperator Operator, Expression Value) : Expression
    {
      public override void Accept(ShaderVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>A function call expression.</summary>
    /// <example>luminance(color.rgb);</example>
    public sealed record FunctionCall(string Name) : Expression
    {
      public ImmutableArray<Parameter> Parameters { get; init; } = ImmutableArray<Parameter>.Empty;

      public override void Accept(ShaderVisitor visitor) => visitor.Visit(this);
    }
  }

  /// <summary>Binary operators used in binary expressions.</summary>
  public enum BinaryOperator
  {
    Add,
    Subtract,
    Multiply,
    Divide,

    Equal,
    NotEqual,
  }

  /// <summary>Unary operators used in unary expressions.</summary>
  public enum UnaryOperator
  {
    Negate,
  }
}

/// <summary>A visitor pattern for <see cref="ShaderSyntaxTree"/> nodes.</summary>
public abstract class ShaderVisitor
{
  public virtual void Visit(ShaderCompilationUnit node)
  {
    foreach (var include in node.Includes)
    {
      include.Accept(this);
    }

    foreach (var constant in node.Constants)
    {
      constant.Accept(this);
    }

    foreach (var uniform in node.Uniforms)
    {
      uniform.Accept(this);
    }

    foreach (var varying in node.Varyings)
    {
      varying.Accept(this);
    }

    foreach (var function in node.Functions)
    {
      function.Accept(this);
    }

    foreach (var stage in node.Stages)
    {
      stage.Accept(this);
    }
  }

  public virtual void Visit(Comment node)
  {
  }

  public virtual void Visit(Include node)
  {
  }

  public virtual void Visit(ShaderTypeDeclaration node)
  {
  }

  public virtual void Visit(UniformDeclaration node)
  {
  }

  public virtual void Visit(VaryingDeclaration node)
  {
  }

  public virtual void Visit(ConstantDeclaration node)
  {
    node.Value.Accept(this);
  }

  public virtual void Visit(VariableDeclaration node)
  {
    node.Value.Accept(this);
  }

  public virtual void Visit(StageDeclaration node)
  {
    foreach (var parameter in node.Parameters)
    {
      parameter.Accept(this);
    }

    foreach (var statement in node.Statements)
    {
      statement.Accept(this);
    }
  }

  public virtual void Visit(FunctionDeclaration node)
  {
    foreach (var parameter in node.Parameters)
    {
      parameter.Accept(this);
    }

    foreach (var statement in node.Statements)
    {
      statement.Accept(this);
    }
  }

  public virtual void Visit(Assignment node)
  {
    node.Value.Accept(this);
  }

  public virtual void Visit(Return node)
  {
    node.Value.Accept(this);
  }

  public virtual void Visit(StatementExpression node)
  {
    node.Body.Accept(this);
  }

  public virtual void Visit(ControlFlow.If node)
  {
    node.Condition.Accept(this);

    foreach (var statement in node.TrueBranch)
    {
      statement.Accept(this);
    }

    foreach (var statement in node.FalseBranch)
    {
      statement.Accept(this);
    }
  }

  public virtual void Visit(ControlFlow.WhileLoop node)
  {
    node.Condition.Accept(this);

    foreach (var statement in node.Statements)
    {
      statement.Accept(this);
    }
  }

  public virtual void Visit(ControlFlow.ForLoop node)
  {
    node.Condition.Accept(this);

    foreach (var statement in node.Statements)
    {
      statement.Accept(this);
    }
  }

  public virtual void Visit(Constant node)
  {
  }

  public virtual void Visit(Symbol node)
  {
  }

  public virtual void Visit(Variadic node)
  {
    foreach (var value in node.Values)
    {
      value.Accept(this);
    }
  }

  public virtual void Visit(Parameter node)
  {
  }

  public virtual void Visit(TypeConstructor node)
  {
    node.Value.Accept(this);
  }

  public virtual void Visit(SampleOperation node)
  {
    node.Value.Accept(this);
  }

  public virtual void Visit(BinaryOperation node)
  {
    node.Left.Accept(this);
    node.Right.Accept(this);
  }

  public virtual void Visit(UnaryOperation node)
  {
    node.Value.Accept(this);
  }

  public virtual void Visit(FunctionCall node)
  {
    foreach (var parameter in node.Parameters)
    {
      parameter.Accept(this);
    }
  }
}
