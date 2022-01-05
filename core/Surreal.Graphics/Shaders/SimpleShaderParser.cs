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

    for (var line = 0;; line++)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var text = await reader.ReadLineAsync();
      if (text == null) break;

      var span = text.Trim().AsStringSpan();

      for (var column = 0; column < span.Length; column++)
      {
        var position = new LinePosition(line + 1, column + 1);
        var token    = ScanToken(span[column], position, span[column..]);

        results.Add(token);
      }
    }

    return results;

    static Token ScanToken(char token, LinePosition position, StringSpan span)
    {
      return token switch
      {
        '(' => new Token(TokenType.LeftParenthesis, position, span[..1]),
        ')' => new Token(TokenType.RightParenthesis, position, span[..1]),
        '{' => new Token(TokenType.LeftBrace, position, span[..1]),
        '}' => new Token(TokenType.RightBrace, position, span[..1]),
        ',' => new Token(TokenType.Comma, position, span[..1]),
        '.' => new Token(TokenType.Dot, position, span[..1]),
        '-' => new Token(TokenType.Minus, position, span[..1]),
        '+' => new Token(TokenType.Plus, position, span[..1]),
        '*' => new Token(TokenType.Star, position, span[..1]),
        '/' => new Token(TokenType.Slash, position, span[..1]),
        ';' => new Token(TokenType.Semicolon, position, span[..1]),

        _ => throw ErrorAt(position, span, $"Unknown token '{token}'"),
      };
    }
  }

  private static Exception ErrorAt(LinePosition position, StringSpan span, string message)
  {
    return new ParseException(position, span, message);
  }

  /// <summary>Different types of tokens recognized by the <see cref="SimpleShaderParser"/>.</summary>
  private enum TokenType
  {
    // text
    WhiteSpace,
    EndOfFile,

    // single character tokens
    LeftParenthesis,
    RightParenthesis,
    LeftBrace,
    RightBrace,
    Comma,
    Dot,
    Minus,
    Plus,
    Star,
    Slash,
    Semicolon,

    // one or two character tokens
    Bang,
    BangEqual,
    Equal,
    EqualEqual,
    Greater,
    GreaterEqual,
    Less,
    LessEqual,

    // literals
    Identifier,
    String,
    Number,

    // TODO: keywords
    Keyword
  }

  /// <summary>Encodes a single token in the <see cref="SimpleShaderParser"/>.</summary>
  private readonly record struct Token(
    TokenType Type,
    LinePosition Position,
    StringSpan Span,
    string? Lexeme = null,
    object? Literal = null
  );

  /// <summary>A position of a token in it's source text.</summary>
  private readonly record struct LinePosition(int Line, int Column)
  {
    public override string ToString() => $"{Line}:{Column}";
  }

  private sealed class ParseException : Exception
  {
    public ParseException(LinePosition position, StringSpan span, string message)
      : base(message)
    {
      Position = position;
      Span     = span;
    }

    public LinePosition Position { get; }
    public StringSpan   Span     { get; }
  }
}
