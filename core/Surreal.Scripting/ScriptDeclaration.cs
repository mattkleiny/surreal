using Surreal.Assets;
using static Surreal.Scripting.ScriptSyntaxTree;
using static Surreal.Scripting.ScriptSyntaxTree.Statement;

namespace Surreal.Scripting;

/// <summary>A declaration for a script and it's parsed AST.</summary>
public sealed record ScriptDeclaration(string Path, CompilationUnit CompilationUnit);

/// <summary>The <see cref="AssetLoader{T}"/> for raw <see cref="ScriptDeclaration"/>s.</summary>
public sealed class ScriptDeclarationLoader : AssetLoader<ScriptDeclaration>
{
  private readonly IScriptParser parser;
  private readonly ImmutableHashSet<string> extensions;
  private readonly Encoding encoding;

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
    this.parser = parser;
    this.extensions = extensions.ToImmutableHashSet();
    this.encoding = encoding;
  }

  /// <summary>The <see cref="IScriptTransformer"/>s to apply to the loaded scripts.</summary>
  public List<IScriptTransformer> Transformers { get; init; } = new();

  public override bool CanHandle(AssetLoaderContext context)
  {
    return base.CanHandle(context) && extensions.Contains(context.Path.Extension);
  }

  public override async ValueTask<ScriptDeclaration> LoadAsync(AssetLoaderContext context, ProgressToken progressToken = default)
  {
    var declaration = await parser.ParseScriptAsync(context.Path, encoding, progressToken.CancellationToken);

    return await TransformScriptAsync(declaration, progressToken.CancellationToken);
  }

  private async Task<ScriptDeclaration> TransformScriptAsync(ScriptDeclaration declaration, CancellationToken cancellationToken = default)
  {
    foreach (var transformer in Transformers)
    {
      if (transformer.CanTransform(declaration))
      {
        declaration = await transformer.TransformAsync(declaration, cancellationToken);
      }
    }

    return declaration;
  }
}

/// <summary>Common AST graph root for all scripting languages.</summary>
public abstract record ScriptSyntaxTree
{
  /// <summary>A compilation unit for all script instructions </summary>
  public sealed record CompilationUnit : ScriptSyntaxTree
  {
    public ImmutableArray<Include>   Includes   { get; init; } = ImmutableArray<Include>.Empty;
    public ImmutableArray<Statement> Statements { get; init; } = ImmutableArray<Statement>.Empty;

    public CompilationUnit MergeWith(CompilationUnit other) => this with
    {
      Includes = Includes.AddRange(other.Includes),
      Statements = Statements.AddRange(other.Statements),
    };
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
