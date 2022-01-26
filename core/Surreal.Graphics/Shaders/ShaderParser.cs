using Surreal.Assets;
using Surreal.Graphics.Shaders.Transformers;
using Surreal.IO;
using Surreal.Text;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree.Expression;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree.Statement;

namespace Surreal.Graphics.Shaders;

/// <summary>Base class for any parser front-end for shader programs.</summary>
public abstract class ShaderParser : Parser<ShaderDeclaration>
{
}

/// <summary>A <see cref="ShaderParser"/> that parses a simple shading language, similar to Godot's language.</summary>
public sealed class StandardShaderParser : ShaderParser
{
  private static ImmutableHashSet<string> Keywords { get; } = new[] { "#include", "#shader_type", "uniform", "varying", "const", "return", "SAMPLE" }.ToImmutableHashSet();
  private static ImmutableHashSet<string> Stages   { get; } = new[] { "vertex", "fragment", "geometry" }.ToImmutableHashSet();

  private readonly IncludeContext includeContext;

  public StandardShaderParser()
    : this(IncludeContext.Standard())
  {
  }

  public StandardShaderParser(IAssetManager manager)
    : this(IncludeContext.FromAssets(manager))
  {
  }

  private StandardShaderParser(IncludeContext includeContext)
  {
    this.includeContext = includeContext;
  }

  /// <summary>The <see cref="IShaderTransformer"/>s to apply to the resultant parsed shader programs.</summary>
  public List<IShaderTransformer> Transformers { get; } = new()
  {
    new SpriteShaderTransformer(),
  };

  public override async ValueTask<ShaderDeclaration> ParseAsync(string path, TextReader reader, CancellationToken cancellationToken = default)
  {
    var tokens  = await TokenizeAsync(Keywords, reader, cancellationToken);
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
        var included = await includeContext.LoadAsync(this, include.Path, cancellationToken);

        compilationUnit = compilationUnit.MergeWith(included.CompilationUnit);
      }
    }

    return compilationUnit;
  }

  /// <summary>Context for syntax parsing operations. This is a recursive descent style parser.</summary>
  private sealed class ShaderParserContext : ParserContext
  {
    public ShaderParserContext(IEnumerable<Token> tokens)
      : base(tokens)
    {
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
  }
}
