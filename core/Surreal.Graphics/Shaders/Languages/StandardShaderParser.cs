using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Surreal.Text;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree.Expression;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree.Statement;

namespace Surreal.Graphics.Shaders.Languages;

/// <summary>A <see cref="IShaderParser"/> that parses a simple shading language, similar to Godot's language.</summary>
public sealed class StandardShaderParser : IShaderParser
{
  private static ImmutableHashSet<string> Keywords        { get; } = new[] { "uniform", "varying", "const", "for", "if", "else" }.ToImmutableHashSet();
  private static ImmutableHashSet<string> CompileKeywords { get; } = new[] { "shader_type", "include" }.ToImmutableHashSet();
  private static ImmutableHashSet<string> StageKeywords   { get; } = new[] { "vertex", "fragment", "geometry" }.ToImmutableHashSet();

  public async ValueTask<ShaderProgramDeclaration> ParseShaderAsync(string path, TextReader reader, CancellationToken cancellationToken = default)
  {
    var tokens  = await TokenizeAsync(reader, cancellationToken);
    var context = new SyntaxParseContext(tokens);

    return new ShaderProgramDeclaration(path, context.ParseCompilationUnit());
  }

  #region Tokenization

  [SuppressMessage("ReSharper", "CognitiveComplexity")]
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

        return new Token(TokenType.Slash, position, span[..1]);
      }

      // strings
      case '"':
      {
        var literal = span.ConsumeUntil('"');

        if (literal[^1] != '"')
          throw ShaderParseException.Create(position, span, $"Unterminated string literal: {literal}");

        return new Token(TokenType.String, position, literal, literal[1..^1].ToString());
      }

      // pre-processor
      case '#' when span.Peek() != '#':
      {
        // recursively scan the rest of this token
        var innerToken = ScanToken(position with { Column = position.Column + 1 }, span[1..]);

        if (innerToken is null)
          throw ShaderParseException.Create(position, span, $"Unrecognized pre-processor {span}");

        var (_, _, innerSpan, innerLiteral) = innerToken.Value;

        if (innerLiteral == null)
          throw ShaderParseException.Create(position, span, $"Unrecognized pre-processor {span}, expected a valid literal");

        if (!CompileKeywords.Contains(innerLiteral.ToString()!))
          throw ShaderParseException.Create(position, span, $"Unrecognized pre-processor keyword {innerLiteral}");

        return new Token(TokenType.CompileKeyword, position, span[..(innerSpan.Length + 1)], innerLiteral);
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

        throw ShaderParseException.Create(position, span, $"Unknown token '{character}'");
      }
    }
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
    CompileKeyword,

    // comments/etc
    Comment,
  }

  /// <summary>Encodes a single token in the <see cref="StandardShaderParser"/>.</summary>
  private readonly record struct Token(TokenType Type, LinePosition Position, StringSpan Span, object? Literal = null);

  /// <summary>A position of a token in it's source text.</summary>
  private readonly record struct LinePosition(int Line, int Column)
  {
    public override string ToString() => $"{Line}:{Column}";
  }

  #endregion

  #region Parsing

  /// <summary>Context for syntax parsing operations. This is a recursive descent style parser.</summary>
  private sealed class SyntaxParseContext
  {
    private readonly Queue<Token> tokens;
    private          Token        lastToken;

    public SyntaxParseContext(Queue<Token> tokens)
    {
      this.tokens = tokens;
    }

    public CompilationUnit ParseCompilationUnit()
    {
      var nodes = new List<ShaderSyntaxTree>();

      while (TryPeek(out var token))
      {
        var node = token.Type switch
        {
          TokenType.CompileKeyword => ParseCompileKeyword(),
          TokenType.Keyword        => ParseKeyword(),
          TokenType.Identifier     => ParseStageOrFunction(),
          _                        => Discard(),
        };

        if (node != null)
        {
          nodes.Add(node);
        }
      }

      return new CompilationUnit
      {
        ShaderType = nodes.OfType<ShaderTypeDeclaration>().FirstOrDefault(new ShaderTypeDeclaration("sprite")),
        Includes   = nodes.OfType<Include>().ToImmutableArray(),
        Uniforms   = nodes.OfType<UniformDeclaration>().ToImmutableArray(),
        Varyings   = nodes.OfType<VaryingDeclaration>().ToImmutableArray(),
        Constants  = nodes.OfType<ConstantDeclaration>().ToImmutableArray(),
        Functions  = nodes.OfType<FunctionDeclaration>().ToImmutableArray(),
        Stages     = nodes.OfType<StageDeclaration>().ToImmutableArray(),
      };
    }

    public ShaderSyntaxTree ParseCompileKeyword()
    {
      var token = Consume(TokenType.CompileKeyword);

      return token.Literal switch
      {
        "include"     => ParseInclude(),
        "shader_type" => ParseShaderTypeDeclaration(),

        _ => throw Error($"An unrecognized compile time keyword was encountered: {token.Literal}"),
      };
    }

    public ShaderSyntaxTree ParseKeyword()
    {
      var token = Consume(TokenType.Keyword);

      return token.Literal switch
      {
        "uniform" => ParseUniformDeclaration(),
        "varying" => ParseVaryingDeclaration(),
        "const"   => ParseConstantDeclaration(),

        _ => throw Error($"An unrecognized keyword was encountered: {token.Literal}"),
      };
    }

    public ShaderSyntaxTree ParseStageOrFunction()
    {
      var returnType = ParsePrimitive();
      var name       = ParseIdentifier();
      var parameters = ParseParameters();
      var statements = ParseStatements();

      if (StageKeywords.Contains(name))
      {
        if (returnType.Type != PrimitiveType.Void)
          throw Error($"The stage function {name} should have a void return type");

        if (parameters.Length > 0)
          throw Error($"The stage function {name} should have no parameters");

        var shaderKind = name switch
        {
          "vertex"   => ShaderKind.Vertex,
          "fragment" => ShaderKind.Fragment,
          "geometry" => ShaderKind.Geometry,

          _ => throw Error($"An unrecognized shader kind was specified {name}"),
        };

        return new StageDeclaration(shaderKind)
        {
          Statements = statements,
        };
      }

      return new FunctionDeclaration(returnType, name)
      {
        Parameters = parameters,
        Statements = statements,
      };
    }

    private ImmutableArray<Parameter> ParseParameters()
    {
      var parameters = ImmutableArray.CreateBuilder<Parameter>();

      Consume(TokenType.LeftParenthesis);

      while (!TryPeek(TokenType.RightParenthesis))
      {
        parameters.Add(ParseParameter());

        if (TryPeek(TokenType.Comma))
        {
          Discard();
        }
      }

      Consume(TokenType.RightParenthesis);

      return parameters.ToImmutable();
    }

    private Parameter ParseParameter()
    {
      var type = ParsePrimitive();
      var name = ParseIdentifier();

      return new Parameter(type, name);
    }

    private ImmutableArray<Statement> ParseStatements()
    {
      var statements = ImmutableArray.CreateBuilder<Statement>();

      Consume(TokenType.LeftBrace);

      while (!TryPeek(TokenType.RightBrace))
      {
        statements.Add(ParseStatement());
      }

      Consume(TokenType.RightBrace);

      return statements.ToImmutable();
    }

    private Statement ParseStatement()
    {
      // TODO: implement me
      var comment = ConsumeLiteral<string>(TokenType.Comment);

      return new Comment(comment);
    }

    private Include ParseInclude()
    {
      var path = ConsumeLiteral<string>(TokenType.String);

      return new Include(path);
    }

    private ShaderTypeDeclaration ParseShaderTypeDeclaration()
    {
      var type = ConsumeLiteral<string>(TokenType.Identifier);

      return new ShaderTypeDeclaration(type);
    }

    private UniformDeclaration ParseUniformDeclaration()
    {
      var type = ParsePrimitive();
      var name = ParseIdentifier();

      return new UniformDeclaration(type, name);
    }

    private VaryingDeclaration ParseVaryingDeclaration()
    {
      var type = ParsePrimitive();
      var name = ParseIdentifier();

      return new VaryingDeclaration(type, name);
    }

    private ConstantDeclaration ParseConstantDeclaration()
    {
      var type  = ParsePrimitive();
      var name  = ParseIdentifier();
      var value = ParseExpression();

      return new ConstantDeclaration(type, name, value);
    }

    private Expression ParseExpression()
    {
      throw new NotImplementedException();
    }

    private Primitive ParsePrimitive()
    {
      var literal = ConsumeLiteral<string>(TokenType.Identifier);

      var type = literal switch
      {
        "void"  => new Primitive(PrimitiveType.Void),
        "bool"  => new Primitive(PrimitiveType.Bool),
        "bool2" => new Primitive(PrimitiveType.Bool, 2),
        "bool3" => new Primitive(PrimitiveType.Bool, 3),
        "bool4" => new Primitive(PrimitiveType.Bool, 4),
        "int"   => new Primitive(PrimitiveType.Int),
        "int2"  => new Primitive(PrimitiveType.Int),
        "int3"  => new Primitive(PrimitiveType.Int, 2),
        "int4"  => new Primitive(PrimitiveType.Int, 3),
        "float" => new Primitive(PrimitiveType.Float),
        "vec2"  => new Primitive(PrimitiveType.Float, 2),
        "vec3"  => new Primitive(PrimitiveType.Float, 3),
        "vec4"  => new Primitive(PrimitiveType.Float, 4),

        _ => throw Error($"An unrecognized primitive type was specified {literal}"),
      };

      return type;
    }

    private string ParseIdentifier()
    {
      return ConsumeLiteral<string>(TokenType.Identifier);
    }

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

    private Token Consume(TokenType type)
    {
      if (!TryPeek(type))
      {
        throw Error($"Expected a token of type {type}");
      }

      return lastToken = tokens.Dequeue();
    }

    private T ConsumeLiteral<T>(TokenType type)
    {
      if (!TryPeek(type))
      {
        throw Error($"Expected a token of type {type}");
      }

      var token = lastToken = tokens.Dequeue();
      if (token.Literal is not T literal)
      {
        throw Error($"Expected a token literal of type {typeof(T)}");
      }

      return literal;
    }

    private ShaderSyntaxTree? Discard()
    {
      lastToken = tokens.Dequeue();

      return null;
    }

    private void DiscardUntil(TokenType type)
    {
      while (!TryPeek(type))
      {
        Discard();
      }
    }

    private Exception Error(string message)
    {
      return ShaderParseException.Create(lastToken, message);
    }
  }

  #endregion

  /// <summary>Indicates an error whilst parsing a program.</summary>
  private sealed class ShaderParseException : Exception
  {
    public ShaderParseException(LinePosition position, StringSpan span, string message)
      : base($"{message} (at {position} in {span})")
    {
      Position = position;
      Span     = span;
    }

    public LinePosition Position { get; }
    public StringSpan   Span     { get; }

    public static Exception Create(LinePosition position, StringSpan span, string message)
    {
      return new ShaderParseException(position, span, message);
    }

    public static Exception Create(Token token, string message)
    {
      return new ShaderParseException(token.Position, token.Span, message);
    }
  }
}
