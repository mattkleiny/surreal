﻿using static Surreal.Scripting.ScriptSyntaxTree;
using static Surreal.Scripting.ScriptSyntaxTree.Statement;

namespace Surreal.Scripting;

/// <summary>
/// A declaration for a script and it's parsed AST.
/// </summary>
public sealed record ScriptDeclaration(string Path, CompilationUnit CompilationUnit);

/// <summary>
/// Common AST graph root for all scripting languages.
/// </summary>
public abstract record ScriptSyntaxTree
{
  /// <summary>
  /// A compilation unit for all script instructions.
  /// </summary>
  public sealed record CompilationUnit : ScriptSyntaxTree
  {
    /// <summary>
    /// The includes for this compilation unit.
    /// </summary>
    public ImmutableArray<Include> Includes { get; init; } = ImmutableArray<Include>.Empty;

    /// <summary>
    /// The top-level statements for this compilation unit.
    /// </summary>
    public ImmutableArray<Statement> Statements { get; init; } = ImmutableArray<Statement>.Empty;

    /// <summary>
    /// Merge this compilation unit with another.
    /// </summary>
    public CompilationUnit MergeWith(CompilationUnit other) => this with
    {
      Includes = Includes.AddRange(other.Includes),
      Statements = Statements.AddRange(other.Statements),
    };
  }

  /// <summary>
  /// A single statement in a shader program.
  /// </summary>
  public abstract record Statement : ScriptSyntaxTree
  {
    /// <summary>A manually placed blank line</summary>
    public sealed record BlankLine : Statement;

    /// <summary>A full-line comment</summary>
    /// <example>// A full-line comment</example>
    public sealed record Comment(string Text) : Statement;

    /// <summary>Includes a module of the given name</summary>
    /// <example>#include math</example>
    public sealed record Include(string Module) : Statement;

    /// <summary>Assigns a value to a variable.</summary>
    /// <example>test = vec3(1,1,1);</example>
    public sealed record Assignment(string Variable, Expression Value) : Statement;

    /// <summary>Declares a new function.</summary>
    public sealed record FunctionDeclaration(string Name, params Statement[] Body) : Statement;
  }

  /// <summary>
  /// A single expression, composite within a larger statement.
  /// </summary>
  public abstract record Expression : ScriptSyntaxTree
  {
    /// <summary>A constant value.</summary>
    /// <example>42</example>
    public sealed record Constant(object Value) : Expression;

    /// <summary>A list of values.</summary>
    /// <example>1, 2, 3</example>
    public sealed record Variadic(params Expression[] Values) : Expression;

    /// <summary>A simple binary operator expression.</summary>
    /// <example>1 + 2</example>
    public sealed record Binary(BinaryOperator Operator, Expression Left, Expression Right) : Expression;

    /// <summary>A simple unary operator expression.</summary>
    /// <example>-4</example>
    public sealed record Unary(UnaryOperator Operator, Expression Value) : Expression;
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
  }

  /// <summary>
  /// Unary operators used in unary expressions.
  /// </summary>
  public enum UnaryOperator
  {
    Negate,
  }
}
