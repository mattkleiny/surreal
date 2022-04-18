using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Surreal.Assets;
using Surreal.IO;

namespace Surreal.Text;

/// <summary>Base class for C-style language parsers with internal tokenization support for common structures and recursive descent parsing.</summary>
public abstract class Parser<T>
{
  public ValueTask<T> ParseAsync(VirtualPath path, CancellationToken cancellationToken = default)
  {
    return ParseAsync(path, Encoding.UTF8, cancellationToken);
  }

  public async ValueTask<T> ParseAsync(VirtualPath path, Encoding encoding, CancellationToken cancellationToken = default)
  {
    await using var stream = await path.OpenInputStreamAsync();

    return await ParseAsync(path.ToString(), stream, encoding, cancellationToken);
  }

  public ValueTask<T> ParseAsync(string path, Stream stream, CancellationToken cancellationToken = default)
  {
    return ParseAsync(path, stream, Encoding.UTF8, cancellationToken);
  }

  public async ValueTask<T> ParseAsync(string path, Stream stream, Encoding encoding, CancellationToken cancellationToken = default)
  {
    using var reader = new StreamReader(stream, encoding);

    return await ParseAsync(path, reader, cancellationToken);
  }

  public async ValueTask<T> ParseAsync(string path, string sourceCode, CancellationToken cancellationToken = default)
  {
    var reader = new StringReader(sourceCode);

    return await ParseAsync(path, reader, cancellationToken);
  }

  public abstract ValueTask<T> ParseAsync(string path, TextReader reader, CancellationToken cancellationToken = default);

  /// <summary>Different kinds of <see cref="Token"/>s that can be parsed.</summary>
  protected enum TokenType
  {
    // single character tokens
    LeftParenthesis,
    RightParenthesis,
    LeftBrace,
    RightBrace,
    LeftBracket,
    RightBracket,
    Comma,
    Dot,
    Minus,
    Plus,
    Star,
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
    Slash,
    SlashEqual,

    // literals
    String,
    Number,
    Identifier,
    Keyword,

    // miscellaneous
    Comment,
  }

  /// <summary>Encodes a single token in the parser.</summary>
  protected readonly record struct Token(
    TokenType Type,
    LinePosition Position,
    StringSpan Span,
    object? Literal = null
  );

  /// <summary>A position of a token in it's source text.</summary>
  protected readonly record struct LinePosition(int Line, int Column)
  {
    public override string ToString() => $"{Line}:{Column}";
  }

  [SuppressMessage("ReSharper", "CognitiveComplexity")]
  protected static async Task<IEnumerable<Token>> TokenizeAsync(ImmutableHashSet<string> keywords, TextReader reader, CancellationToken cancellationToken)
  {
    var results = new List<Token>();

    for (var line = 0;; line++)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var text = await reader.ReadLineAsync();
      if (text == null) break;

      var span = text.AsStringSpan();

      for (var column = 0; column < span.Length; column++)
      {
        var position = new LinePosition(line + 1, column + 1);
        var token = ScanToken(keywords, position, span[column..]);

        if (token != null)
        {
          results.Add(token.Value);

          // skip forward on long spans
          if (token.Value.Span.Length > 1)
            column += token.Value.Span.Length - 1;
        }
      }
    }

    return results;

    // Scans a single token from the given string span
    static Token? ScanToken(ImmutableHashSet<string> keywords, LinePosition position, StringSpan span)
    {
      var character = span[0];

      switch (character)
      {
        // simple characters
        case '(': return new Token(TokenType.LeftParenthesis, position, span[..1]);
        case ')': return new Token(TokenType.RightParenthesis, position, span[..1]);
        case '{': return new Token(TokenType.LeftBrace, position, span[..1]);
        case '}': return new Token(TokenType.RightBrace, position, span[..1]);
        case '[': return new Token(TokenType.LeftBracket, position, span[..1]);
        case ']': return new Token(TokenType.RightBracket, position, span[..1]);
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
            return new Token(TokenType.BangEqual, position, span[..2]);

          return new Token(TokenType.Bang, position, span[..1]);
        }
        case '=':
        {
          if (span.Match('='))
            return new Token(TokenType.EqualEqual, position, span[..2]);

          return new Token(TokenType.Equal, position, span[..1]);
        }
        case '<':
        {
          if (span.Match('='))
            return new Token(TokenType.LessEqual, position, span[..2]);

          return new Token(TokenType.Less, position, span[..1]);
        }
        case '>':
        {
          if (span.Match('='))
            return new Token(TokenType.GreaterEqual, position, span[..2]);

          return new Token(TokenType.Greater, position, span[..1]);
        }
        case '/':
        {
          if (span.Match('/'))
            return new Token(TokenType.Comment, position, span, span[2..].ToString().Trim());

          if (span.Match('='))
            return new Token(TokenType.SlashEqual, position, span[..2]);

          return new Token(TokenType.Slash, position, span[..1]);
        }

        // strings
        case '"':
        {
          var literal = span.ConsumeUntil('"');

          if (literal[^1] != '"')
            throw new ParseException(position, span, $"Unterminated string literal: {literal}");

          return new Token(TokenType.String, position, literal, literal[1..^1].ToString());
        }

        default:
        {
          // whitespace
          if (char.IsWhiteSpace(character))
            return null; // ignore whitespace

          // numbers
          if (char.IsDigit(character))
          {
            var number = span.ConsumeNumericWithFractions();

            return new Token(TokenType.Number, position, number, decimal.Parse(number.ToString(), CultureInfo.InvariantCulture));
          }

          // identifiers and keywords
          if (char.IsLetter(character) || character is '_' or '#')
          {
            var identifier = span.ConsumeAlphaNumeric();
            var literal = identifier.ToString();

            if (keywords.Contains(literal))
            {
              return new Token(TokenType.Keyword, position, identifier, literal);
            }

            return new Token(TokenType.Identifier, position, identifier, literal);
          }

          throw new ParseException(position, span, $"Unknown token '{character}'");
        }
      }
    }
  }

  /// <summary>Indicates an error whilst parsing.</summary>
  protected sealed class ParseException : Exception
  {
    public ParseException(Token token, string message)
      : this(token.Position, token.Span, message)
    {
    }

    public ParseException(LinePosition position, StringSpan span, string message)
      : base($"{message} (at {position} in {span})")
    {
      Position = position;
      Span = span;
    }

    public LinePosition Position { get; }
    public StringSpan   Span     { get; }
  }

  /// <summary>Base class for a recursive descent parse context.</summary>
  protected abstract class ParserContext
  {
    private readonly Queue<Token> tokens;
    private Token lastToken;

    protected ParserContext(IEnumerable<Token> tokens)
    {
      this.tokens = new Queue<Token>(tokens);
    }

    protected bool TryPeek(out Token token)
    {
      return tokens.TryPeek(out token);
    }

    protected bool TryPeek(TokenType type)
    {
      if (TryPeek(out var token))
      {
        return token.Type == type;
      }

      return false;
    }

    protected bool TryConsume(TokenType type)
    {
      if (TryPeek(out var token) && token.Type == type)
      {
        tokens.Dequeue();
        return true;
      }

      return false;
    }

    private bool TryConsume(TokenType type, out Token token)
    {
      if (TryPeek(out token) && token.Type == type)
      {
        return true;
      }

      return false;
    }

    protected Token Consume(TokenType type, string errorMessage = "An unexpected token was encountered")
    {
      if (!TryPeek(type))
      {
        throw Error(errorMessage);
      }

      return Consume();
    }

    protected Token Consume()
    {
      return lastToken = tokens.Dequeue();
    }

    protected bool TryConsumeLiteral<TLiteral>(TokenType type, out TLiteral result)
    {
      if (TryPeek(out var token) && token.Type == type && token.Literal is TLiteral literal)
      {
        lastToken = tokens.Dequeue();
        result = literal;

        return true;
      }

      result = default!;
      return false;
    }

    protected bool TryConsumeLiteralIf<TLiteral>(TokenType type, TLiteral comparison)
    {
      if (TryPeek(out var token) && token.Type == type && token.Literal is TLiteral literal)
      {
        if (Equals(literal, comparison))
        {
          lastToken = tokens.Dequeue();
          return true;
        }
      }

      return false;
    }

    protected TLiteral ConsumeLiteral<TLiteral>(TokenType type)
    {
      if (!TryConsumeLiteral(type, out TLiteral literal))
      {
        throw Error($"Expected a token of type {type}");
      }

      return literal;
    }

    protected Exception Error(string message)
    {
      return new ParseException(lastToken, message);
    }
  }

  /// <summary>A delegate which loads a value from some inclusion source.</summary>
  protected delegate ValueTask<T> IncludeHandler(Parser<T> parser, VirtualPath path, CancellationToken cancellationToken = default);

  /// <summary>Commonly used <see cref="IncludeHandler"/>s.</summary>
  protected static class IncludeHandlers
  {
    /// <summary>A standard <see cref="IncludeHandler"/> that delegates back to the given <see cref="Parser{T}"/> and caches the result internally.</summary>
    public static IncludeHandler Static()
    {
      var includesByPath = new ConcurrentDictionary<VirtualPath, T>();

      return async (parser, path, cancellationToken) =>
      {
        if (!includesByPath.TryGetValue(path, out var declaration))
        {
          includesByPath[path] = declaration = await parser.ParseAsync(path, cancellationToken);
        }

        return declaration;
      };
    }

    /// <summary>A <see cref="IncludeHandler"/> implementation that delegates back to the asset system via the given <see cref="IAssetManager"/>.</summary>
    public static IncludeHandler FromAssets(IAssetManager manager)
    {
      return async (_, path, cancellationToken) =>
      {
        return await manager.LoadAssetAsync<T>(path, cancellationToken);
      };
    }
  }
}
