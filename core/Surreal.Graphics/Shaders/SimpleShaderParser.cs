using Surreal.Text;

namespace Surreal.Graphics.Shaders;

/// <summary>A <see cref="IShaderParser"/> that parses a simple shading language, similar to Godot's language.</summary>
public sealed class SimpleShaderParser : IShaderParser
{
  public async Task<ShaderProgramDeclaration> ParseShaderAsync(string name, TextReader reader, CancellationToken cancellationToken = default)
  {
    var tokens = await TokenizeAsync(reader, cancellationToken);

    return new ShaderProgramDeclaration(name, string.Empty, ShaderArchetype.Sprite);
  }

  private static async Task<IEnumerable<Token>> TokenizeAsync(TextReader reader, CancellationToken cancellationToken)
  {
    var results = new List<Token>();

    for (var lineNo = 0;; lineNo++)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var text = await reader.ReadLineAsync();
      if (text == null) break;

      var line = text.Trim().AsStringSpan();

      for (var columnNo = 0; columnNo < line.Length; columnNo++)
      {
        var span     = line[columnNo..];
        var position = new LinePosition(lineNo + 1, columnNo + 1);

        // TODO: implement tokenization

        results.Add(new Token(TokenType.WhiteSpace, position, span));
      }
    }

    return results;
  }

  /// <summary>Different types of tokens recognized by the <see cref="SimpleShaderParser"/>.</summary>
  private enum TokenType
  {
    WhiteSpace,
    Comment,
    Identifier,
    Keyword,
    Number,
  }

  /// <summary>Encodes a single token in the <see cref="SimpleShaderParser"/>.</summary>
  private readonly record struct Token(
    TokenType Type,
    LinePosition Position,
    StringSpan Span,
    string? Lexeme = null
  );

  /// <summary>A position of a token in it's source text.</summary>
  private readonly record struct LinePosition(int Line, int Column)
  {
    public override string ToString() => $"{Line}:{Column}";
  }
}
