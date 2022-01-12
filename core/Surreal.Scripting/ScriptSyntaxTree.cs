using System.Diagnostics.CodeAnalysis;
using Surreal.Assets;
using static Surreal.Scripting.ScriptSyntaxTree;
using static Surreal.Scripting.ScriptSyntaxTree.Statement;

namespace Surreal.Scripting;

/// <summary>A declaration for a script and it's parsed AST.</summary>
public sealed record ScriptDeclaration(string Path, CompilationUnit CompilationUnit);

/// <summary>The <see cref="AssetLoader{T}"/> for raw <see cref="ScriptDeclaration"/>s.</summary>
public sealed class ScriptDeclarationLoader : AssetLoader<ScriptDeclaration>
{
  private readonly IScriptParser            parser;
  private readonly ImmutableHashSet<string> extensions;
  private readonly Encoding                 encoding;

  public ScriptDeclarationLoader(IScriptParser parser, params string[] extensions)
    : this(parser, extensions.AsEnumerable())
  {
  }

  public ScriptDeclarationLoader(IScriptParser parser, IEnumerable<string> extensions)
    : this(parser, extensions, Encoding.UTF8)
  {
  }

  public ScriptDeclarationLoader(IScriptParser parser, IEnumerable<string> extensions, Encoding encoding)
  {
    this.parser     = parser;
    this.extensions = extensions.ToImmutableHashSet();
    this.encoding   = encoding;
  }

  public override bool CanHandle(AssetLoaderContext context)
  {
    return base.CanHandle(context) && extensions.Contains(context.Path.Extension);
  }

  public override async ValueTask<ScriptDeclaration> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken = default)
  {
    return await parser.ParseScriptAsync(context.Path, encoding, cancellationToken);
  }
}

/// <summary>Common AST graph root for all scripting languages.</summary>
public abstract record ScriptSyntaxTree
{
  /// <summary>A compilation unit for all script instructions </summary>
  public sealed record CompilationUnit : ScriptSyntaxTree
  {
    public CompilationUnit(params ScriptSyntaxTree[] nodes)
      : this(nodes.AsEnumerable())
    {
    }

    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public CompilationUnit(IEnumerable<ScriptSyntaxTree> nodes)
    {
      Includes  = nodes.OfType<Include>().ToImmutableArray();
      Functions = nodes.OfType<FunctionDeclaration>().ToImmutableArray();
    }

    public ImmutableArray<Include>             Includes  { get; init; }
    public ImmutableArray<FunctionDeclaration> Functions { get; init; }
  }

  /// <summary>A single statement in a shader program.</summary>
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

  /// <summary>A single expression, composite within a larger statement.</summary>
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
