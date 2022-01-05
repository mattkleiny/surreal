namespace Surreal.Graphics.Shaders;

/// <summary>Different kinds of <see cref="Primitive"/>s.</summary>
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

/// <summary>A type declaration with optional cardinality for vector representations.</summary>
public readonly record struct Primitive(
  PrimitiveType Type,
  int? Cardinality = null,
  Precision? Precision = null)
{
  public static implicit operator Primitive(PrimitiveType type) => new(type);
}

/// <summary>Base class for the shader program instruction AST.</summary>
public abstract record ShaderInstruction
{
  /// <summary>A single statement, in a single line and terminated.</summary>
  public abstract record Statement : ShaderInstruction
  {
    /// <summary>A manually placed blank line</summary>
    public sealed record BlankLine : Statement;

    /// <summary>A full-line comment</summary>
    /// <example>// A full-line comment</example>
    public sealed record Comment(string Text) : Statement;

    /// <summary>Includes a module of the given name</summary>
    /// <example>#include math</example>
    public sealed record Include(string Module) : Statement;

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

    /// <summary>Declares a standard function.</summary>
    /// <example>float circle(float radius) { ... }</example>
    public sealed record FunctionDeclaration(Primitive ReturnType, string Name, params ShaderInstruction[] Body) : Statement;

    /// <summary>Standard control flow variants.</summary>
    public abstract record ControlFlow : Statement
    {
      /// <summary>Checks if a value is true or false.</summary>
      /// <example>if (test) { ... }</example>
      public sealed record If(Expression value, params ShaderInstruction[] Body) : ControlFlow;
    }
  }

  /// <summary>A single expression, composite within a larger statement.</summary>
  public abstract record Expression : ShaderInstruction
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
    Sub,
    Mul,
    Div,
  }

  /// <summary>Unary operators used in unary expressions.</summary>
  public enum UnaryOperator
  {
    Negate,
  }
}
