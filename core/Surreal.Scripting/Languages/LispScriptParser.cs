using Surreal.Text;

namespace Surreal.Scripting.Languages;

/// <summary>A <see cref="IScriptParser"/> for Lisp programs.</summary>
public sealed class LispScriptParser : IScriptParser
{
  public async ValueTask<ScriptDeclaration> ParseScriptAsync(string path, TextReader reader, CancellationToken cancellationToken = default)
  {
    var expressions = await SymbolicExpression.Parse(reader, cancellationToken);

    throw new NotImplementedException();
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
          .Split(' ');

        return new Queue<string>(tokens);
      }

      var tokens = Tokenize(span);

      throw new NotImplementedException();
    }

    private static SymbolicExpression ParseLeaf(string literal)
    {
      if (float.TryParse(literal, out var single)) return new Leaf(single);
      if (int.TryParse(literal, out var integer)) return new Leaf(integer);

      return new Leaf(literal);
    }

    /// <summary>A node of other <see cref="SymbolicExpression"/> nodes.</summary>
    public record Node(ImmutableArray<SymbolicExpression> Contents) : SymbolicExpression
    {
      public Node(IEnumerable<SymbolicExpression> expressions)
        : this(ImmutableArray.CreateRange(expressions))
      {
      }
    }

    /// <summary>A single <see cref="SymbolicExpression"/> leaf.</summary>
    public record Leaf(object Value) : SymbolicExpression;
  }
}
