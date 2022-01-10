using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Surreal.Text;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree;

namespace Surreal.Graphics.Shaders;

/// <summary>A <see cref="IShaderParser"/> that parses a simple shading language, similar to Godot's language.</summary>
public sealed class StandardShaderParser : IShaderParser
{
  private static ImmutableHashSet<string> Keywords { get; } = new[] { "for", "if", "else" }.ToImmutableHashSet();

  public async ValueTask<ShaderProgramDeclaration> ParseShaderAsync(string path, TextReader reader, CancellationToken cancellationToken = default)
  {
    var tokens          = await TokenizeAsync(reader, cancellationToken);
    var compilationUnit = ParseCompilationUnit(new ParseContext(tokens));

    return new ShaderProgramDeclaration(path, ShaderArchetype.Sprite, compilationUnit);
  }

  private static CompilationUnit ParseCompilationUnit(ParseContext context)
  {
    var nodes = new List<ShaderSyntaxTree>();

    // TODO: recursive decent parser

    return new CompilationUnit(nodes);
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
        var position = new LinePosition(line + 1, column + 1);
        var token    = ScanToken(position, span[column..]);

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
  [SuppressMessage("ReSharper", "CognitiveComplexity")]
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

          return new Token(TokenType.Number, position, number, decimal.Parse(number.ToString(), CultureInfo.InvariantCulture));
        }

        // identifiers and keywords
        if (char.IsLetter(character) || character == '_')
        {
          var identifier = span.ConsumeAlphaNumeric();
          var literal    = identifier.ToString();

          if (Keywords.Contains(literal))
          {
            return new Token(TokenType.Keyword, position, identifier, literal);
          }

          return new Token(TokenType.Identifier, position, identifier, literal);
        }

        throw ErrorAt(position, span, $"Unknown token '{character}'");
      }
    }
  }

  private static Exception ErrorAt(LinePosition position, StringSpan span, string message)
  {
    return new ParseException(position, span, message);
  }

  /// <summary>Different types of tokens recognized by the parser.</summary>
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

  /// <summary>Encodes a single token in the <see cref="StandardShaderParser"/>.</summary>
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

  /// <summary>Context for parsing operations.</summary>
  private sealed class ParseContext
  {
    private readonly Queue<Token> tokens;

    public ParseContext(Queue<Token> tokens)
    {
      this.tokens = tokens;
    }

    public bool TryPeek(out Token token)
    {
      return tokens.TryPeek(out token);
    }

    public bool TryDequeue(out Token token)
    {
      return tokens.TryDequeue(out token);
    }

    /// <summary>Attempts to consume a single token from the remaining tokens.</summary>
    public bool TryConsume(TokenType type, out Token result)
    {
      if (TryPeek(out var token))
      {
        if (token.Type == type)
        {
          result = tokens.Dequeue();
          return true;
        }
      }

      result = default;
      return false;
    }
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
