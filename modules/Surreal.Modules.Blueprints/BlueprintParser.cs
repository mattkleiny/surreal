using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Surreal.Assets;
using Surreal.IO;
using Surreal.Text;
using static Surreal.BlueprintSyntaxTree;

namespace Surreal;

/// <summary>Parser front-end for blueprint descriptors.</summary>
public sealed class BlueprintParser
{
  private static ImmutableHashSet<string> Keywords { get; } = new[] { "#include", "component", "attribute", "event", "#tag", "entity", "item", "override" }.ToImmutableHashSet();

  private readonly Environment environment;

  public BlueprintParser()
    : this(Environment.Standard())
  {
  }

  public BlueprintParser(IAssetManager manager)
    : this(Environment.FromAssets(manager))
  {
  }

  private BlueprintParser(Environment environment)
  {
    this.environment = environment;
  }

  public ValueTask<BlueprintDeclaration> ParseAsync(VirtualPath path, CancellationToken cancellationToken = default)
  {
    return ParseAsync(path, Encoding.UTF8, cancellationToken);
  }

  public async ValueTask<BlueprintDeclaration> ParseAsync(VirtualPath path, Encoding encoding, CancellationToken cancellationToken = default)
  {
    await using var stream = await path.OpenInputStreamAsync();

    return await ParseAsync(path.ToString(), stream, encoding, cancellationToken);
  }

  public ValueTask<BlueprintDeclaration> ParseAsync(string path, Stream stream, CancellationToken cancellationToken = default)
  {
    return ParseAsync(path, stream, Encoding.UTF8, cancellationToken);
  }

  public async ValueTask<BlueprintDeclaration> ParseAsync(string path, Stream stream, Encoding encoding, CancellationToken cancellationToken = default)
  {
    using var reader = new StreamReader(stream, encoding);

    return await ParseAsync(path, reader, cancellationToken);
  }

  public async ValueTask<BlueprintDeclaration> ParseAsync(string path, string sourceCode, CancellationToken cancellationToken = default)
  {
    var reader = new StringReader(sourceCode);

    return await ParseAsync(path, reader, cancellationToken);
  }

  public async ValueTask<BlueprintDeclaration> ParseAsync(string path, TextReader reader, CancellationToken cancellationToken = default)
  {
    var tokens  = await TokenizeAsync(reader, cancellationToken);
    var context = new BlueprintParserContext(path, tokens);

    var declaration = context.ParseBlueprintDeclaration();

    declaration = await MergeIncludesAsync(declaration, cancellationToken);

    // TODO: validate the result?

    return declaration;
  }

  private async Task<BlueprintDeclaration> MergeIncludesAsync(BlueprintDeclaration declaration, CancellationToken cancellationToken = default)
  {
    var includedPaths = new HashSet<VirtualPath>();

    foreach (var include in declaration.Includes)
    {
      if (includedPaths.Add(include.Path))
      {
        var included = await environment.LoadBlueprintAsync(this, include.Path, cancellationToken);

        declaration = declaration.MergeWith(included);
      }
    }

    return declaration;
  }

  /// <summary>Different kinds of <see cref="Token"/>s that can be parsed.</summary>
  private enum TokenType
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

  /// <summary>Encodes a single token in the <see cref="BlueprintParser"/>.</summary>
  private readonly record struct Token(TokenType Type, LinePosition Position, StringSpan Span, object? Literal = null);

  /// <summary>A position of a token in it's source text.</summary>
  private readonly record struct LinePosition(int Line, int Column)
  {
    public override string ToString() => $"{Line}:{Column}";
  }

  [SuppressMessage("ReSharper", "CognitiveComplexity")]
  private static async Task<IEnumerable<Token>> TokenizeAsync(TextReader reader, CancellationToken cancellationToken)
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
        var token    = ScanToken(position, span[column..]);

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
    static Token? ScanToken(LinePosition position, StringSpan span)
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
            throw new BlueprintParseException(position, span, $"Unterminated string literal: {literal}");

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
            var literal    = identifier.ToString();

            if (Keywords.Contains(literal))
            {
              return new Token(TokenType.Keyword, position, identifier, literal);
            }

            return new Token(TokenType.Identifier, position, identifier, literal);
          }

          throw new BlueprintParseException(position, span, $"Unknown token '{character}'");
        }
      }
    }
  }

  /// <summary>Context for syntax parsing operations. This is a recursive descent style parser.</summary>
  private sealed class BlueprintParserContext
  {
    private readonly string       path;
    private readonly Queue<Token> tokens;

    private Token lastToken;

    public BlueprintParserContext(string path, IEnumerable<Token> tokens)
    {
      this.path   = path;
      this.tokens = new Queue<Token>(tokens);
    }

    public BlueprintDeclaration ParseBlueprintDeclaration()
    {
      var nodes = new List<BlueprintSyntaxTree>();

      while (TryPeek(out var token))
      {
        var node = token.Type switch
        {
          TokenType.Keyword => ParseGlobalKeyword(),
          _                 => ParseNull(),
        };

        if (node != null)
        {
          nodes.Add(node);
        }
      }

      return new BlueprintDeclaration(path)
      {
        Includes   = nodes.OfType<IncludeStatement>().ToImmutableArray(),
        Archetypes = nodes.OfType<BlueprintArchetype>().ToImmutableArray(),
      };
    }

    private BlueprintSyntaxTree ParseGlobalKeyword()
    {
      var literal = ConsumeLiteral<string>(TokenType.Keyword);

      return literal switch
      {
        "#include" => ParseIncludeStatement(),
        "item"     => ParseArchetypeDeclaration(BlueprintArchetypeKind.Item),
        "entity"   => ParseArchetypeDeclaration(BlueprintArchetypeKind.Entity),

        _ => throw Error($"An unrecognized keyword was encountered: {literal}"),
      };
    }

    private IncludeStatement ParseIncludeStatement()
    {
      var path = ConsumeLiteral<string>(TokenType.String);

      Consume(TokenType.SemiColon);

      return new IncludeStatement(path);
    }

    private BlueprintArchetype ParseArchetypeDeclaration(BlueprintArchetypeKind kind)
    {
      var name      = ConsumeLiteral<string>(TokenType.Identifier);
      var baseTypes = ConsumeBaseTypeList();
      var block     = ConsumeStatementBlock();

      return new BlueprintArchetype(kind, name)
      {
        BaseTypes  = baseTypes,
        Tags       = block.OfType<TagDeclaration>().ToImmutableArray(),
        Attributes = block.OfType<AttributeDeclaration>().ToImmutableArray(),
        Components = block.OfType<ComponentDeclaration>().ToImmutableArray(),
        Events     = block.OfType<EventDeclaration>().ToImmutableArray(),
      };
    }

    private ImmutableArray<string> ConsumeBaseTypeList()
    {
      var baseTypes = ImmutableArray.CreateBuilder<string>();

      if (TryConsume(TokenType.Colon))
      {
        while (!TryPeek(TokenType.LeftBrace))
        {
          baseTypes.Add(ConsumeLiteral<string>(TokenType.Identifier));

          TryConsume(TokenType.Comma);
        }
      }

      return baseTypes.ToImmutable();
    }

    private List<BlueprintSyntaxTree> ConsumeStatementBlock()
    {
      var results = new List<BlueprintSyntaxTree>();

      Consume(TokenType.LeftBrace);

      while (!TryPeek(TokenType.RightBrace))
      {
        if (TryConsume(TokenType.Comment)) continue; // ignore comments

        results.Add(ParseLocalDeclaration());
      }

      Consume(TokenType.RightBrace);

      return results;
    }

    private BlueprintSyntaxTree ParseLocalDeclaration()
    {
      var literal = ConsumeLiteral<string>(TokenType.Keyword);

      return literal switch
      {
        "#tag"      => ParseTagDeclaration(),
        "attribute" => ParseAttributeDeclaration(),
        "component" => ParseComponentDeclaration(),
        "event"     => ParseEventDeclaration(),

        _ => throw Error($"An unrecognized keyword was encountered: {literal}"),
      };
    }

    private TagDeclaration ParseTagDeclaration()
    {
      var name = ConsumeLiteral<string>(TokenType.Identifier);

      return new TagDeclaration(name);
    }

    private AttributeDeclaration ParseAttributeDeclaration()
    {
      var name = ConsumeLiteral<string>(TokenType.Identifier);

      var parameters = ParseParameterList();
      var isOverride = ParseOverride();

      Consume(TokenType.SemiColon);

      return new AttributeDeclaration(name)
      {
        IsOverride = isOverride,
        Parameters = parameters,
      };
    }

    private ComponentDeclaration ParseComponentDeclaration()
    {
      var name = ConsumeLiteral<string>(TokenType.Identifier);

      var parameters = ParseParameterList();
      var isOverride = ParseOverride();

      Consume(TokenType.SemiColon);

      return new ComponentDeclaration(name)
      {
        IsOverride = isOverride,
        Parameters = parameters,
      };
    }

    private EventDeclaration ParseEventDeclaration()
    {
      var name       = ConsumeLiteral<string>(TokenType.Identifier);
      var parameters = ParseParameterList();

      Consume(TokenType.SemiColon);

      return new EventDeclaration(name)
      {
        Parameters = parameters,
      };
    }

    private ImmutableArray<Expression> ParseParameterList()
    {
      var parameters = ImmutableArray.CreateBuilder<Expression>();

      Consume(TokenType.LeftParenthesis);

      while (!TryPeek(TokenType.RightParenthesis))
      {
        if (TryConsumeLiteral(TokenType.String, out string @string))
        {
          parameters.Add(new Expression.Constant(@string));
          TryConsume(TokenType.Comma);
        }
        else if (TryConsumeLiteral(TokenType.Number, out decimal number))
        {
          parameters.Add(new Expression.Constant(number));
          TryConsume(TokenType.Comma);
        }
        else if (TryConsumeLiteral(TokenType.Identifier, out string identifier))
        {
          parameters.Add(new Expression.Identifier(identifier));
          TryConsume(TokenType.Comma);
        }
        else
        {
          throw Error("An unexpected parameter was encountered");
        }
      }

      Consume(TokenType.RightParenthesis);

      return parameters.ToImmutable();
    }

    private bool ParseOverride()
    {
      return TryConsumeLiteralIf(TokenType.Keyword, "override");
    }

    private BlueprintSyntaxTree? ParseNull()
    {
      Consume();

      return null;
    }

    #region Token Helpers

    private bool TryPeek(out Token token)
    {
      return tokens.TryPeek(out token);
    }

    private bool TryPeek(TokenType type)
    {
      if (TryPeek(out var token))
      {
        return token.Type == type;
      }

      return false;
    }

    private bool TryConsume(TokenType type)
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

    private Token Consume(TokenType type, string errorMessage = "An unexpected token was encountered")
    {
      if (!TryPeek(type))
      {
        throw Error(errorMessage);
      }

      return Consume();
    }

    private Token Consume()
    {
      return lastToken = tokens.Dequeue();
    }

    private bool TryConsumeLiteral<T>(TokenType type, out T result)
    {
      if (TryPeek(out var token) && token.Type == type && token.Literal is T literal)
      {
        lastToken = tokens.Dequeue();
        result    = literal;

        return true;
      }

      result = default!;
      return false;
    }

    private bool TryConsumeLiteralIf<T>(TokenType type, T comparison)
    {
      if (TryPeek(out var token) && token.Type == type && token.Literal is T literal)
      {
        if (Equals(literal, comparison))
        {
          lastToken = tokens.Dequeue();
          return true;
        }
      }

      return false;
    }

    private T ConsumeLiteral<T>(TokenType type)
    {
      if (!TryConsumeLiteral(type, out T literal))
      {
        throw Error($"Expected a token of type {type}");
      }

      return literal;
    }

    #endregion

    private Exception Error(string message)
    {
      return new BlueprintParseException(lastToken, message);
    }
  }

  /// <summary>Indicates an error whilst parsing a blueprint.</summary>
  private sealed class BlueprintParseException : Exception
  {
    public BlueprintParseException(Token token, string message)
      : this(token.Position, token.Span, message)
    {
    }

    public BlueprintParseException(LinePosition position, StringSpan span, string message)
      : base($"{message} (at {position} in {span})")
    {
      Position = position;
      Span     = span;
    }

    public LinePosition Position { get; }
    public StringSpan   Span     { get; }
  }

  /// <summary>Environmental access for the <see cref="BlueprintParser"/>.</summary>
  private abstract class Environment
  {
    public static Environment Standard()                        => new DefaultEnvironment();
    public static Environment FromAssets(IAssetManager manager) => new AssetEnvironment(manager);

    /// <summary>Loads the given related shader back through the parsing pipeline pipeline.</summary>
    public abstract ValueTask<BlueprintDeclaration> LoadBlueprintAsync(BlueprintParser parser, VirtualPath path, CancellationToken cancellationToken = default);

    /// <summary>A standard <see cref="Environment"/> that delegates back to the given <see cref="ShaderParser"/> and caches the result internally.</summary>
    private sealed class DefaultEnvironment : Environment
    {
      private readonly ConcurrentDictionary<VirtualPath, BlueprintDeclaration> declarationsByPath = new();

      public override async ValueTask<BlueprintDeclaration> LoadBlueprintAsync(BlueprintParser parser, VirtualPath path, CancellationToken cancellationToken = default)
      {
        if (!declarationsByPath.TryGetValue(path, out var declaration))
        {
          declarationsByPath[path] = declaration = await parser.ParseAsync(path, cancellationToken);
        }

        return declaration;
      }
    }

    /// <summary>A <see cref="Environment"/> implementation that delegates back to the asset system via the given <see cref="IAssetManager"/>.</summary>
    private sealed class AssetEnvironment : Environment
    {
      private readonly IAssetManager manager;

      public AssetEnvironment(IAssetManager manager)
      {
        this.manager = manager;
      }

      public override async ValueTask<BlueprintDeclaration> LoadBlueprintAsync(BlueprintParser parser, VirtualPath path, CancellationToken cancellationToken = default)
      {
        return await manager.LoadAssetAsync<BlueprintDeclaration>(path);
      }
    }
  }
}
