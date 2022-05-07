using Surreal.Assets;
using Surreal.Graphics.Shaders.Transformers;
using Surreal.IO;
using Surreal.Text;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree.Expression;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree.Statement;

namespace Surreal.Graphics.Shaders;

// TODO: remove self-recursive tree evaluations (depth + 1, maxDepth, etc).

/// <summary>A <see cref="Parser{T}"/> that parses a simple shading language, similar to Godot's shader language.</summary>
public sealed class ShaderParser : Parser<ShaderDeclaration>
{
  private static ImmutableHashSet<string> Keywords   { get; } = ImmutableHashSet.Create("#include", "#shader_type", "uniform", "varying", "const", "return", "SAMPLE");
  private static ImmutableHashSet<string> Primitives { get; } = ImmutableHashSet.Create("void", "bool", "bool2", "bool3", "bool4", "int", "int2", "int3", "int4", "float", "float2", "float3", "float4", "vec2", "vec3", "vec4", "sampler1d", "sampler2d", "sampler3d", "lowp", "mediump", "highp");
  private static ImmutableHashSet<string> Stages     { get; } = ImmutableHashSet.Create("vertex", "fragment", "geometry");

  private readonly IncludeHandler includeHandler;

  public ShaderParser()
    : this(IncludeHandlers.Static())
  {
  }

  public ShaderParser(IAssetManager manager)
    : this(IncludeHandlers.FromAssets(manager))
  {
  }

  private ShaderParser(IncludeHandler includeHandler)
  {
    this.includeHandler = includeHandler;
  }

  /// <summary>The <see cref="IShaderTransformer"/>s to apply to the resultant parsed shader programs.</summary>
  public List<IShaderTransformer> Transformers { get; } = new()
  {
    new SpriteShaderTransformer()
  };

  public override async ValueTask<ShaderDeclaration> ParseAsync(string path, TextReader reader, CancellationToken cancellationToken = default)
  {
    var tokens = await TokenizeAsync(Keywords, Primitives, reader, cancellationToken);
    var context = new ShaderParserContext(tokens);

    // parse the main compilation unit
    var compilationUnit = context.ParseCompilationUnit();

    // transform parsed shaders and merge in included results (n.b order is important)
    compilationUnit = ApplyTransformers(compilationUnit);
    compilationUnit = await MergeIncludedShadersAsync(compilationUnit, cancellationToken);

    return new ShaderDeclaration(path, compilationUnit);
  }

  private ShaderCompilationUnit ApplyTransformers(ShaderCompilationUnit compilationUnit)
  {
    foreach (var transformer in Transformers)
    {
      if (transformer.CanTransform(compilationUnit))
      {
        compilationUnit = transformer.Transform(compilationUnit);
      }
    }

    return compilationUnit;
  }

  private async Task<ShaderCompilationUnit> MergeIncludedShadersAsync(ShaderCompilationUnit compilationUnit, CancellationToken cancellationToken)
  {
    var includedPaths = new HashSet<VirtualPath>();

    foreach (var include in compilationUnit.Includes)
    {
      if (includedPaths.Add(include.Path))
      {
        var included = await includeHandler(this, include.Path, cancellationToken);

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

    public ShaderCompilationUnit ParseCompilationUnit()
    {
      var nodes = new List<ShaderSyntaxTree>();

      while (TryPeek(out var token))
      {
        var node = token.Type switch
        {
          TokenType.Keyword   => ParseKeyword(),
          TokenType.Primitive => ParseFunction(),
          _                   => ParseNull()
        };

        if (node != null)
        {
          nodes.Add(node);
        }
      }

      return new ShaderCompilationUnit
      {
        ShaderType = nodes.OfType<ShaderTypeDeclaration>().FirstOrDefault(new ShaderTypeDeclaration("none")),
        Includes   = nodes.OfType<Include>().ToImmutableHashSet(),
        Uniforms   = nodes.OfType<UniformDeclaration>().ToImmutableArray(),
        Varyings   = nodes.OfType<VaryingDeclaration>().ToImmutableArray(),
        Constants  = nodes.OfType<ConstantDeclaration>().ToImmutableArray(),
        Functions  = nodes.OfType<FunctionDeclaration>().ToImmutableArray(),
        Stages     = nodes.OfType<StageDeclaration>().ToImmutableArray()
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

        _ => throw Error($"An unrecognized keyword was encountered: {literal}")
      };
    }

    private Statement ParseFunction()
    {
      var returnType = ParsePrimitive();
      var name = ParseIdentifier();
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

          _ => throw Error($"An unrecognized shader kind was specified {name}")
        };

        return new StageDeclaration(shaderKind)
        {
          Parameters = parameters,
          Statements = statements
        };
      }

      return new FunctionDeclaration(returnType, name)
      {
        Parameters = parameters,
        Statements = statements
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

          _ => throw Error($"An unrecognized keyword was encountered: {keyword}")
        };
      }

      if (TryConsumeLiteral(TokenType.Identifier, out string variable))
      {
        Consume(TokenType.Equal);

        var value = ParseExpression();
        var assignment = new Assignment(variable, value);

        Consume(TokenType.SemiColon);

        return assignment;
      }

      if (TryPeek(TokenType.Primitive))
      {
        var declaration = ParseVariableDeclaration();

        Consume(TokenType.SemiColon);

        return declaration;
      }

      return new StatementExpression(ParseExpression());
    }

    private VariableDeclaration ParseVariableDeclaration()
    {
      throw new NotImplementedException();
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
      throw new NotImplementedException();
    }

    private VaryingDeclaration ParseVaryingDeclaration()
    {
      throw new NotImplementedException();
    }

    private ConstantDeclaration ParseConstantDeclaration()
    {
      throw new NotImplementedException();
    }

    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
    private Expression ParseExpression(int depth = 0, int maxDepth = 32)
    {
      if (depth > maxDepth) throw Error("Exceeded max depth");

      if (TryConsumeLiteral(TokenType.Number, out decimal number))
        return new Constant(number);

      if (TryConsumeLiteral(TokenType.String, out string text))
        return new Constant(text);

      if (TryPeek(TokenType.Primitive))
      {
        var type = ParsePrimitive();
        var value = ParseExpression(depth + 1, maxDepth);

        return new TypeConstructor(type, value);
      }

      if (TryParseUnaryOperator(out var unaryOperator))
      {
        var value = ParseExpression(depth + 1, maxDepth);

        return new UnaryOperation(unaryOperator, value);
      }

      // TODO: remove self-recursive instantiation
      // TODO: break this down into smaller rules
      var expression =
        TryConsumeLiteralIf(TokenType.Keyword, "SAMPLE")
          ? ParseSampleOperation()
          : TryConsumeLiteral(TokenType.Identifier, out string identifier)
            ? new Symbol(identifier)
            : ParseExpression(depth + 1, maxDepth);

      if (TryParseBinaryOperator(out var binaryOperator))
      {
        var value = ParseExpression(depth + 1, maxDepth);

        return new BinaryOperation(binaryOperator, expression, value);
      }

      return expression;
    }

    private Primitive ParsePrimitive()
    {
      var precision = default(Precision?);
      var literal = ConsumeLiteral<string>(TokenType.Primitive);

      if (literal is "lowp" or "medp" or "highp")
      {
        precision = literal switch
        {
          "lowp"  => Precision.Low,
          "medp"  => Precision.Medium,
          "highp" => Precision.High,

          _ => throw Error($"An unrecognized precision was specified {literal}")
        };

        literal = ConsumeLiteral<string>(TokenType.Primitive);
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

        _ => throw Error($"An unrecognized primitive type was specified {literal}")
      };

      return type;
    }

    private SampleOperation ParseSampleOperation()
    {
      Consume(TokenType.LeftParenthesis);
      var name = ParseIdentifier();
      Consume(TokenType.Comma);
      var value = ParseExpression();
      Consume(TokenType.RightParenthesis);

      return new SampleOperation(name, value);
    }

    private string ParseIdentifier()
    {
      return ConsumeLiteral<string>(TokenType.Identifier);
    }

    private bool TryParseUnaryOperator(out UnaryOperator result)
    {
      if (TryConsume(TokenType.Minus))
      {
        result = UnaryOperator.Negate;
        return true;
      }

      result = default;
      return false;
    }

    private bool TryParseBinaryOperator(out BinaryOperator result)
    {
      if (TryConsume(TokenType.Plus))
      {
        result = BinaryOperator.Add;
        return true;
      }

      if (TryConsume(TokenType.Minus))
      {
        result = BinaryOperator.Subtract;
        return true;
      }

      if (TryConsume(TokenType.Star))
      {
        result = BinaryOperator.Multiply;
        return true;
      }

      if (TryConsume(TokenType.Slash))
      {
        result = BinaryOperator.Divide;
        return true;
      }

      if (TryConsume(TokenType.Equal))
      {
        result = BinaryOperator.Equal;
        return true;
      }

      if (TryConsume(TokenType.BangEqual))
      {
        result = BinaryOperator.NotEqual;
        return true;
      }

      result = default;
      return false;
    }

    private ShaderSyntaxTree? ParseNull()
    {
      Consume();

      return null;
    }
  }
}
