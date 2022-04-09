using Surreal.Text;
using static Surreal.Scripting.ScriptSyntaxTree;
using static Surreal.Scripting.ScriptSyntaxTree.Statement;

namespace Surreal.Scripting.Languages;

/// <summary>A <see cref="IScriptParser"/> for Lisp programs.</summary>
public sealed class LispScriptParser : IScriptParser
{
  public async ValueTask<ScriptDeclaration> ParseScriptAsync(string path, TextReader reader, CancellationToken cancellationToken = default)
  {
    var expressions = await SymbolicExpression.Parse(reader, cancellationToken);
    var context = new LispParserContext(expressions);

    var compilationUnit = context.ParseCompilationUnit();

    return new ScriptDeclaration(path, compilationUnit);
  }

  /// <summary>A context for parsing <see cref="SymbolicExpression"/>s to the <see cref="ScriptSyntaxTree"/>.</summary>
  private sealed class LispParserContext
  {
    private readonly Queue<SymbolicExpression> expressions;

    public LispParserContext(IEnumerable<SymbolicExpression> expressions)
    {
      this.expressions = new Queue<SymbolicExpression>(expressions);
    }

    public CompilationUnit ParseCompilationUnit()
    {
      var nodes = new List<ScriptSyntaxTree>();

      while (expressions.TryPeek(out _))
      {
        nodes.Add(ParseStatement());
      }

      return new CompilationUnit
      {
        Includes = nodes.OfType<Include>().ToImmutableArray(),
        Statements = nodes.OfType<Statement>().ToImmutableArray(),
      };
    }

    public Statement ParseStatement()
    {
      if (expressions.TryDequeue(out var expression))
      {
        switch (expression)
        {
          case SymbolicExpression.Atom(var atom):
            throw new LispParseException("An unexpected atom was encountered");

          case SymbolicExpression.Node(var node):
            throw new LispParseException("An unexpected node was encountered");
        }
      }

      throw new NotImplementedException();
    }
  }

  /// <summary>A symbolic expression (or s-expression), for use in recursive deconstruction of the LISP tree.</summary>
  private abstract record SymbolicExpression
  {
    public static async Task<IEnumerable<SymbolicExpression>> Parse(TextReader reader, CancellationToken cancellationToken)
    {
      var results = new List<SymbolicExpression>();

      while (true)
      {
        cancellationToken.ThrowIfCancellationRequested();

        var line = await reader.ReadLineAsync();
        if (line == null) break;

        var expression = Parse(line);

        results.Add(expression);
      }

      return results;
    }

    private static SymbolicExpression Parse(StringSpan span)
    {
      static Queue<string> Tokenize(StringSpan span)
      {
        var tokens = span.ToString()
          // add spaces around parenthesis for splitting
          .Replace("(", " ( ")
          .Replace(")", " ) ")
          .Split()
          .Where(token => token != "");

        return new Queue<string>(tokens);
      }

      var tokens = Tokenize(span);

      return ParseNode(tokens);
    }

    private static SymbolicExpression ParseNode(Queue<string> tokens)
    {
      var token = tokens.Dequeue();

      if (token == ")")
      {
        throw new LispParseException("Unexpected ')' character");
      }

      if (token == "(")
      {
        var builder = ImmutableArray.CreateBuilder<SymbolicExpression>();

        while (tokens.TryPeek(out token) && token != ")")
        {
          builder.Add(ParseNode(tokens));
        }

        return new Node(builder.ToImmutable());
      }

      return new Atom(token);
    }

    private static SymbolicExpression ParseAtom(string literal)
    {
      if (int.TryParse(literal, out var integer))
        return new Atom(integer);

      if (float.TryParse(literal, out var single))
        return new Atom(single);

      return new Atom(literal);
    }

    /// <summary>A node of other <see cref="SymbolicExpression"/> nodes.</summary>
    public record Node(ImmutableArray<SymbolicExpression> Contents) : SymbolicExpression;

    /// <summary>A single <see cref="SymbolicExpression"/> leaf.</summary>
    public record Atom(object Value) : SymbolicExpression;
  }

  /// <summary>Indicates an error whilst parsing a lisp program.</summary>
  private sealed class LispParseException : Exception
  {
    public LispParseException(string message)
      : base(message)
    {
    }
  }
}
