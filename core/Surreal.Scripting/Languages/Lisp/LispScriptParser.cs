using Surreal.Text;

namespace Surreal.Scripting.Languages.Lisp;

/// <summary>A <see cref="IScriptParser"/> for Lisp programs.</summary>
public sealed class LispScriptParser : IScriptParser
{
  public async ValueTask<ScriptDeclaration> ParseScriptAsync(string path, TextReader reader, CancellationToken cancellationToken = default)
  {
    var expressions = await SymbolicExpression.Parse(reader, cancellationToken);

    throw new NotImplementedException();
  }

  /// <summary>An s-expression, for use in partial deconstruction of LISP expressions.</summary>
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

    private static SymbolicExpression Parse(string line)
    {
      var tokens = Tokenize(line);

      throw new NotImplementedException();
    }

    private static SymbolicExpression ParseAtom(string literal)
    {
      if (float.TryParse(literal, out var single)) return new Atom(single);
      if (int.TryParse(literal, out var integer)) return new Atom(integer);

      return new Atom(literal);
    }

    private static Queue<string> Tokenize(StringSpan span)
    {
      var tokens = span.ToString()
        // add spaces around parenthesis for splitting
        .Replace("(", " ( ")
        .Replace(")", " ) ")
        .Split(' ', StringSplitOptions.RemoveEmptyEntries);

      return new Queue<string>(tokens);
    }

    /// <summary>A single <see cref="SymbolicExpression"/> leaf</summary>
    public record Atom(object Value) : SymbolicExpression;

    /// <summary>A list of <see cref="SymbolicExpression"/> nodes.</summary>
    public record List(ImmutableArray<SymbolicExpression> Contents) : SymbolicExpression
    {
      public List(IEnumerable<SymbolicExpression> expressions)
        : this(ImmutableArray.CreateRange(expressions))
      {
      }
    }
  }
}
