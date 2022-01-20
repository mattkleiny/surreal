﻿using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Surreal.Assets;
using Surreal.Graphics.Shaders.Transformers;
using Surreal.IO;
using Surreal.Text;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree.Expression;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree.Statement;

namespace Surreal.Graphics.Shaders;

/// <summary>Base class for any parser front-end for shader programs.</summary>
public abstract class ShaderParser
{
  public ValueTask<ShaderDeclaration> ParseShaderAsync(VirtualPath path, CancellationToken cancellationToken = default)
  {
    return ParseShaderAsync(path, Encoding.UTF8, cancellationToken);
  }

  public async ValueTask<ShaderDeclaration> ParseShaderAsync(VirtualPath path, Encoding encoding, CancellationToken cancellationToken = default)
  {
    await using var stream = await path.OpenInputStreamAsync();

    return await ParseShaderAsync(path.ToString(), stream, encoding, cancellationToken);
  }

  public ValueTask<ShaderDeclaration> ParseShaderAsync(string path, Stream stream, CancellationToken cancellationToken = default)
  {
    return ParseShaderAsync(path, stream, Encoding.UTF8, cancellationToken);
  }

  public async ValueTask<ShaderDeclaration> ParseShaderAsync(string path, Stream stream, Encoding encoding, CancellationToken cancellationToken = default)
  {
    using var reader = new StreamReader(stream, encoding);

    return await ParseShaderAsync(path, reader, cancellationToken);
  }

  public async ValueTask<ShaderDeclaration> ParseShaderAsync(string path, string sourceCode, CancellationToken cancellationToken = default)
  {
    var reader = new StringReader(sourceCode);

    return await ParseShaderAsync(path, reader, cancellationToken);
  }

  public abstract ValueTask<ShaderDeclaration> ParseShaderAsync(string path, TextReader reader, CancellationToken cancellationToken = default);

  /// <summary>Environmental access for the <see cref="ShaderParser"/>.</summary>
  public abstract class Environment
  {
    public static Environment Standard()                        => new DefaultEnvironment();
    public static Environment FromAssets(IAssetManager manager) => new AssetEnvironment(manager);

    /// <summary>Loads the given related shader back through the parsing pipeline pipeline.</summary>
    public abstract ValueTask<ShaderDeclaration> LoadShaderAsync(ShaderParser parser, VirtualPath path, CancellationToken cancellationToken = default);

    /// <summary>A standard <see cref="Environment"/> that delegates back to the given <see cref="ShaderParser"/> and caches the result internally.</summary>
    private sealed class DefaultEnvironment : Environment
    {
      private readonly ConcurrentDictionary<VirtualPath, ShaderDeclaration> declarationsByPath = new();

      public override async ValueTask<ShaderDeclaration> LoadShaderAsync(ShaderParser parser, VirtualPath path, CancellationToken cancellationToken = default)
      {
        if (!declarationsByPath.TryGetValue(path, out var declaration))
        {
          declarationsByPath[path] = declaration = await parser.ParseShaderAsync(path, cancellationToken);
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

      public override async ValueTask<ShaderDeclaration> LoadShaderAsync(ShaderParser parser, VirtualPath path, CancellationToken cancellationToken = default)
      {
        return await manager.LoadAssetAsync<ShaderDeclaration>(path);
      }
    }
  }
}

/// <summary>A <see cref="ShaderParser"/> that parses a simple shading language, similar to Godot's language.</summary>
public sealed class StandardShaderParser : ShaderParser
{
  private static ImmutableHashSet<string> Keywords { get; } = new[] { "#include", "#shader_type", "uniform", "varying", "const", "return", "SAMPLE" }.ToImmutableHashSet();
  private static ImmutableHashSet<string> Stages   { get; } = new[] { "vertex", "fragment", "geometry" }.ToImmutableHashSet();

  private readonly Environment environment;

  public StandardShaderParser()
    : this(Environment.Standard())
  {
  }

  public StandardShaderParser(Environment environment)
  {
    this.environment = environment;
  }

  /// <summary>The <see cref="IShaderTransformer"/>s to apply to the resultant parsed shader programs.</summary>
  public List<IShaderTransformer> Transformers { get; } = new()
  {
    new SpriteShaderTransformer(),
  };

  public override async ValueTask<ShaderDeclaration> ParseShaderAsync(string path, TextReader reader, CancellationToken cancellationToken = default)
  {
    var tokens  = await TokenizeAsync(reader, cancellationToken);
    var context = new ShaderParserContext(tokens);

    // parse the main compilation unit
    var compilationUnit = context.ParseCompilationUnit();

    // transform parsed shaders and merge in included results (n.b order is important)
    compilationUnit = await ApplyTransformersAsync(compilationUnit, cancellationToken);
    compilationUnit = await MergeIncludedShadersAsync(compilationUnit, cancellationToken);

    return new ShaderDeclaration(path, compilationUnit);
  }

  private async Task<CompilationUnit> ApplyTransformersAsync(CompilationUnit compilationUnit, CancellationToken cancellationToken)
  {
    foreach (var transformer in Transformers)
    {
      if (transformer.CanTransform(compilationUnit))
      {
        compilationUnit = await transformer.TransformAsync(compilationUnit, cancellationToken);
      }
    }

    return compilationUnit;
  }

  private async Task<CompilationUnit> MergeIncludedShadersAsync(CompilationUnit compilationUnit, CancellationToken cancellationToken)
  {
    var includedPaths = new HashSet<VirtualPath>();

    foreach (var include in compilationUnit.Includes)
    {
      if (includedPaths.Add(include.Path))
      {
        var included = await environment.LoadShaderAsync(this, include.Path, cancellationToken);

        compilationUnit = compilationUnit.MergeWith(included.CompilationUnit);
      }
    }

    return compilationUnit;
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

    // miscellaneous
    Comment,
  }

  /// <summary>Encodes a single token in the <see cref="StandardShaderParser"/>.</summary>
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
            throw new ShaderParseException(position, span, $"Unterminated string literal: {literal}");

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

          throw new ShaderParseException(position, span, $"Unknown token '{character}'");
        }
      }
    }
  }

  /// <summary>Context for syntax parsing operations. This is a recursive descent style parser.</summary>
  private sealed class ShaderParserContext
  {
    private readonly Queue<Token> tokens;
    private          Token        lastToken;

    public ShaderParserContext(IEnumerable<Token> tokens)
    {
      this.tokens = new Queue<Token>(tokens);
    }

    public CompilationUnit ParseCompilationUnit()
    {
      var nodes = new List<ShaderSyntaxTree>();

      while (TryPeek(out var token))
      {
        var node = token.Type switch
        {
          TokenType.Keyword    => ParseKeyword(),
          TokenType.Identifier => ParseFunction(),
          _                    => ParseNull(),
        };

        if (node != null)
        {
          nodes.Add(node);
        }
      }

      return new CompilationUnit
      {
        ShaderType = nodes.OfType<ShaderTypeDeclaration>().FirstOrDefault(new ShaderTypeDeclaration("none")),
        Includes   = nodes.OfType<Include>().ToImmutableHashSet(),
        Uniforms   = nodes.OfType<UniformDeclaration>().ToImmutableArray(),
        Varyings   = nodes.OfType<VaryingDeclaration>().ToImmutableArray(),
        Constants  = nodes.OfType<ConstantDeclaration>().ToImmutableArray(),
        Functions  = nodes.OfType<FunctionDeclaration>().ToImmutableArray(),
        Stages     = nodes.OfType<StageDeclaration>().ToImmutableArray(),
      };
    }

    private ShaderSyntaxTree ParseKeyword()
    {
      var literal = ConsumeLiteral<string>(TokenType.Keyword);

      return literal switch
      {
        "#include"     => ParseInclude(),
        "#shader_type" => ParseShaderTypeDeclaration(),
        "uniform"      => ParseUniformDeclaration(),
        "varying"      => ParseVaryingDeclaration(),
        "const"        => ParseConstantDeclaration(),
        "SAMPLE"       => ParseSampleOperation(),

        _ => throw Error($"An unrecognized keyword was encountered: {literal}"),
      };
    }

    private Statement ParseFunction()
    {
      var returnType = ParsePrimitive();
      var name       = ParseIdentifier();
      var parameters = ParseParameters();
      var statements = ParseStatements();

      if (Stages.Contains(name))
      {
        if (returnType.Type != PrimitiveType.Void)
          throw Error($"The stage function {name} should have a void return type");

        var shaderKind = name switch
        {
          "vertex"   => ShaderKind.Vertex,
          "fragment" => ShaderKind.Fragment,
          "geometry" => ShaderKind.Geometry,

          _ => throw Error($"An unrecognized shader kind was specified {name}"),
        };

        return new StageDeclaration(shaderKind)
        {
          Parameters = parameters,
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
          Consume();
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
      if (TryConsumeLiteral(TokenType.Comment, out string comment))
        return new Comment(comment);

      if (TryConsumeLiteral(TokenType.Keyword, out string keyword))
      {
        return keyword switch
        {
          "if"     => ParseIfStatement(),
          "while"  => ParseWhileStatement(),
          "for"    => ParseForStatement(),
          "return" => ParseReturnStatement(),

          _ => throw Error($"An unrecognized keyword was encountered: {keyword}"),
        };
      }

      var expression = ParseExpression();

      Consume(TokenType.SemiColon, "Expect a semicolon after an expression");

      return new StatementExpression(expression);
    }

    private Statement ParseIfStatement()
    {
      throw new NotImplementedException();
    }

    private Statement ParseWhileStatement()
    {
      throw new NotImplementedException();
    }

    private Statement ParseForStatement()
    {
      throw new NotImplementedException();
    }

    private Statement ParseReturnStatement()
    {
      return new Return(ParseExpression());
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
      if (TryConsumeLiteral(TokenType.Number, out decimal number))
        return new Constant(number);

      if (TryConsumeLiteral(TokenType.String, out string value))
        return new Constant(value);

      if (TryConsumeLiteral(TokenType.Identifier, out string symbol))
        return new Symbol(symbol);

      throw new NotImplementedException();
    }

    private Primitive ParsePrimitive()
    {
      var precision = default(Precision?);
      var literal   = ConsumeLiteral<string>(TokenType.Identifier);

      if (literal is "lowp" or "medp" or "highp")
      {
        precision = literal switch
        {
          "lowp"  => Precision.Low,
          "medp"  => Precision.Medium,
          "highp" => Precision.High,

          _ => throw Error($"An unrecognized precision was specified {literal}"),
        };

        literal = ConsumeLiteral<string>(TokenType.Identifier);
      }

      var type = literal switch
      {
        "void"      => new Primitive(PrimitiveType.Void, null, precision),
        "bool"      => new Primitive(PrimitiveType.Bool, null, precision),
        "bool2"     => new Primitive(PrimitiveType.Bool, 2, precision),
        "bool3"     => new Primitive(PrimitiveType.Bool, 3, precision),
        "bool4"     => new Primitive(PrimitiveType.Bool, 4, precision),
        "int"       => new Primitive(PrimitiveType.Int, null, precision),
        "int2"      => new Primitive(PrimitiveType.Int, 2, precision),
        "int3"      => new Primitive(PrimitiveType.Int, 3, precision),
        "int4"      => new Primitive(PrimitiveType.Int, 4, precision),
        "float"     => new Primitive(PrimitiveType.Float, null, precision),
        "float2"    => new Primitive(PrimitiveType.Float, 2, precision),
        "float3"    => new Primitive(PrimitiveType.Float, 3, precision),
        "float4"    => new Primitive(PrimitiveType.Float, 4, precision),
        "vec2"      => new Primitive(PrimitiveType.Float, 2, precision),
        "vec3"      => new Primitive(PrimitiveType.Float, 3, precision),
        "vec4"      => new Primitive(PrimitiveType.Float, 4, precision),
        "sampler1d" => new Primitive(PrimitiveType.Sampler, 1),
        "sampler2d" => new Primitive(PrimitiveType.Sampler, 2),
        "sampler3d" => new Primitive(PrimitiveType.Sampler, 3),

        _ => throw Error($"An unrecognized primitive type was specified {literal}"),
      };

      return type;
    }

    private string ParseIdentifier()
    {
      return ConsumeLiteral<string>(TokenType.Identifier);
    }

    private SampleOperation ParseSampleOperation()
    {
      var name  = ParseIdentifier();
      var value = ParseExpression();

      return new SampleOperation(name, value);
    }

    private UnaryOperator ParseUnaryOperator()
    {
      if (TryConsume(TokenType.Minus)) return UnaryOperator.Negate;

      throw Error("An unrecognized token was encountered");
    }

    private BinaryOperator ParseBinaryOperator()
    {
      if (TryConsume(TokenType.Plus)) return BinaryOperator.Add;
      if (TryConsume(TokenType.Minus)) return BinaryOperator.Subtract;
      if (TryConsume(TokenType.Star)) return BinaryOperator.Multiply;
      if (TryConsume(TokenType.Slash)) return BinaryOperator.Divide;
      if (TryConsume(TokenType.Equal)) return BinaryOperator.Equal;
      if (TryConsume(TokenType.BangEqual)) return BinaryOperator.NotEqual;

      throw Error("An unrecognized token was encountered");
    }

    private ShaderSyntaxTree? ParseNull()
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
      if (TryPeek(out var token))
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
      if (TryPeek(out var token) && token.Literal is T literal)
      {
        lastToken = tokens.Dequeue();
        result    = literal;

        return true;
      }

      result = default!;
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

    private Exception Error(string message) => new ShaderParseException(lastToken, message);
  }

  /// <summary>Indicates an error whilst parsing a program.</summary>
  private sealed class ShaderParseException : Exception
  {
    public ShaderParseException(Token token, string message)
      : this(token.Position, token.Span, message)
    {
    }

    public ShaderParseException(LinePosition position, StringSpan span, string message)
      : base($"{message} (at {position} in {span})")
    {
      Position = position;
      Span     = span;
    }

    public LinePosition Position { get; }
    public StringSpan   Span     { get; }
  }
}
