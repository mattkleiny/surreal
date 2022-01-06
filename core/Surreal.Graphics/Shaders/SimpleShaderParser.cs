using Surreal.Collections;
using Surreal.Text;

namespace Surreal.Graphics.Shaders;

/// <summary>A <see cref="IShaderParser"/> that parses a simple shading language, similar to Godot's language.</summary>
public sealed class SimpleShaderParser : IShaderParser
{
  private static HashSet<string> Keywords { get; } = new() { "for", "if", "else" };

  public async Task<ShaderProgramDeclaration> ParseShaderAsync(string name, TextReader reader, CancellationToken cancellationToken = default)
  {
    var tokens = await TokenizeAsync(reader, cancellationToken);

    var description  = ParseDescription(tokens);
    var declarations = ParseDeclarations(tokens);

    return new ShaderProgramDeclaration(name, description, ShaderArchetype.Sprite);
  }

  private static string ParseDescription(Queue<Token> tokens)
  {
    var builder = new StringBuilder();

    while (tokens.TryPeekAndDequeue(_ => _.Type == TokenType.Comment, out var token))
    {
      builder.AppendLine(token.Literal?.ToString()?.Trim());
    }

    return builder.ToString();
  }

  private static IEnumerable<ShaderDeclaration> ParseDeclarations(Queue<Token> tokens)
  {
    // TODO: implement me

    yield break;
  }

  private static async Task<Queue<Token>> TokenizeAsync(TextReader reader, CancellationToken cancellationToken)
  {
    var results = new Queue<Token>();

    for (var line = 0;; line++)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var text = await reader.ReadLineAsync();
      if (text == null) break;

      var span = text.AsStringSpan();

      for (var column = 0; column < span.Length; column++)
      {
        var token = ScanToken(new(line + 1, column + 1), span[column..]);
        if (token != null)
        {
          results.Enqueue(token.Value);

          // skip forward on long spans
          if (token.Value.Span.Length > 1)
            column += token.Value.Span.Length - 1;
        }
      }
    }

    return results;
  }

  /// <summary>Scans a token from the given <see cref="StringSpan"/>.</summary>
  private static Token? ScanToken(LinePosition position, StringSpan span)
  {
    var character = span[0];

    switch (character)
    {
      // simple characters
      case '(': return new Token(TokenType.LeftParenthesis, position, span[..1]);
      case ')': return new Token(TokenType.RightParenthesis, position, span[..1]);
      case '{': return new Token(TokenType.LeftBrace, position, span[..1]);
      case '}': return new Token(TokenType.RightBrace, position, span[..1]);
      case ',': return new Token(TokenType.Comma, position, span[..1]);
      case '.': return new Token(TokenType.Dot, position, span[..1]);
      case '-': return new Token(TokenType.Minus, position, span[..1]);
      case '+': return new Token(TokenType.Plus, position, span[..1]);
      case '*': return new Token(TokenType.Star, position, span[..1]);
      case ':': return new Token(TokenType.Colon, position, span[..1]);
      case ';': return new Token(TokenType.SemiColon, position, span[..1]);

      // dual characters
      case '!':
      {
        if (span.Match('='))
        {
          return new Token(TokenType.BangEqual, position, span[..2]);
        }

        return new Token(TokenType.Bang, position, span[..1]);
      }
      case '=':
      {
        if (span.Match('='))
        {
          return new Token(TokenType.EqualEqual, position, span[..2]);
        }

        return new Token(TokenType.Equal, position, span[..1]);
      }
      case '<':
      {
        if (span.Match('='))
        {
          return new Token(TokenType.LessEqual, position, span[..2]);
        }

        return new Token(TokenType.Less, position, span[..1]);
      }
      case '>':
      {
        if (span.Match('='))
        {
          return new Token(TokenType.GreaterEqual, position, span[..2]);
        }

        return new Token(TokenType.Greater, position, span[..1]);
      }
      case '/':
      {
        if (span.Match('/'))
        {
          return new Token(TokenType.Comment, position, span, span[2..]);
        }

        return new Token(TokenType.Slash, position, span[..1]);
      }

      // strings
      case '"':
      {
        var literal = span.ConsumeUntil('"');

        if (literal[^1] != '"')
        {
          throw ErrorAt(position, span, $"Unterminated string literal: {literal}");
        }

        return new Token(TokenType.String, position, literal, literal[1..^1].ToString());
      }

      default:
      {
        // whitespace
        if (char.IsWhiteSpace(character))
        {
          // ignore whitespace
          return null;
        }

        // numbers
        if (char.IsDigit(character))
        {
          var number = span.ConsumeNumericWithFractions();

          return new Token(TokenType.Number, position, number, decimal.Parse(number.ToString()!));
        }

        // identifiers and keywords
        if (char.IsLetter(character) || character == '_')
        {
          var identifier = span.ConsumeAlphaNumeric();

          if (Keywords.Contains(identifier.ToString()!))
          {
            return new Token(TokenType.Keyword, position, identifier);
          }

          return new Token(TokenType.Identifier, position, identifier, identifier.ToString());
        }

        throw ErrorAt(position, span, $"Unknown token '{character}'");
      }
    }
  }

  private static Exception ErrorAt(LinePosition position, StringSpan span, string message)
  {
    return new ParseException(position, span, message);
  }

  /// <summary>Different types of tokens recognized by the <see cref="SimpleShaderParser"/>.</summary>
  private enum TokenType
  {
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
    Colon,
    SemiColon,

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
    String,
    Number,
    Identifier,
    Keyword,

    // comments/etc
    Comment,
  }

  /// <summary>Encodes a single token in the <see cref="SimpleShaderParser"/>.</summary>
  private readonly record struct Token(
    TokenType Type,
    LinePosition Position,
    StringSpan Span,
    object? Literal = null
  );

  /// <summary>A position of a token in it's source text.</summary>
  private readonly record struct LinePosition(int Line, int Column)
  {
    public override string ToString() => $"{Line}:{Column}";
  }

  /// <summary>Indicates an error whilst parsing a program.</summary>
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
