using System.Diagnostics.CodeAnalysis;
using OpenTK.Graphics.OpenGL;
using Surreal.Graphics.Shaders;
using Surreal.Internal.Graphics.Resources;
using Surreal.IO;
using Surreal.Text;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree.Statement;
using PrimitiveType = Surreal.Graphics.Shaders.PrimitiveType;

namespace Surreal.Internal.Graphics;

/// <summary>
/// The <see cref="IShaderCompiler"/> for OpenTK.
/// <para/>
/// This implementation transpiles shader programs into GLSL code, for later compilation.
/// by the OpenGL driver (using the standard glShader* method).
/// </summary>
internal sealed class OpenTKShaderCompiler : IShaderCompiler
{
  private readonly string version;

  public OpenTKShaderCompiler(string version = "330")
  {
    this.version = version;
  }

  public async ValueTask<ICompiledShaderProgram> CompileAsync(
    IShaderCompilerEnvironment environment,
    ShaderDeclaration declaration,
    CancellationToken cancellationToken = default
  )
  {
    var shaders = ImmutableArray.CreateBuilder<OpenTKShader>(declaration.CompilationUnit.Stages.Length);

    foreach (var stage in declaration.CompilationUnit.Stages)
    {
      var context = new ShaderCompileContext(version, stage.Kind, environment);

      await context.CompileStageAsync(declaration, stage, cancellationToken);

      var shaderType = ConvertShaderKind(stage.Kind);
      var shaderCode = context.FinaliseToString();

      shaders.Add(new OpenTKShader(shaderType, shaderCode));
    }

    return new OpenTKShaderSet(declaration.Path, shaders.MoveToImmutable());
  }

  private static ShaderType ConvertShaderKind(ShaderKind kind) => kind switch
  {
    ShaderKind.Vertex   => ShaderType.VertexShader,
    ShaderKind.Geometry => ShaderType.GeometryShader,
    ShaderKind.Fragment => ShaderType.FragmentShader,

    _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
  };

  /// <summary>Context for compiling a single shader program.</summary>
  private sealed class ShaderCompileContext
  {
    private readonly string                     version;
    private readonly ShaderKind                 shaderKind;
    private readonly IShaderCompilerEnvironment environment;
    private readonly ShaderCodeBuilder          builder;
    private readonly HashSet<VirtualPath>       includedPaths = new();

    public ShaderCompileContext(string version, ShaderKind shaderKind, IShaderCompilerEnvironment environment)
    {
      this.version     = version;
      this.shaderKind  = shaderKind;
      this.environment = environment;

      builder = new ShaderCodeBuilder(new StringBuilder());
    }

    public string FinaliseToString()
    {
      return builder.StringBuilder.ToString();
    }

    public async ValueTask CompileStageAsync(ShaderDeclaration declaration, StageDeclaration stage, CancellationToken cancellationToken = default)
    {
      // compile preamble and include other modules
      CompilePreamble(declaration);

      // compile the top-level compilation unit
      await CompileUnitAsync(declaration.CompilationUnit, cancellationToken);

      // compile stage local
      CompileStatement(stage);
    }

    private async ValueTask CompileUnitAsync(CompilationUnit compilationUnit, CancellationToken cancellationToken = default)
    {
      // link included compilation units
      if (compilationUnit.Includes.Length > 0)
      {
        await CompileIncludesAsync(compilationUnit.Includes, cancellationToken);
      }

      // compile globals (N.B: order matters)
      CompileStatements(compilationUnit.Uniforms);
      CompileStatements(compilationUnit.Varyings);
      CompileStatements(compilationUnit.Constants);
      CompileStatements(compilationUnit.Functions);
    }

    private void CompilePreamble(ShaderDeclaration declaration)
    {
      builder.AppendComment($"Compiled from: {declaration.Path}");
      builder.AppendLine($"#version {version}");
      builder.AppendLine();
    }

    private async ValueTask CompileIncludesAsync(ImmutableArray<Include> includes, CancellationToken cancellationToken)
    {
      foreach (var include in includes)
      {
        // only include modules once
        if (includedPaths.Add(include.Path))
        {
          var program = await environment.ExpandShaderAsync(include.Path, cancellationToken);

          await CompileUnitAsync(program.CompilationUnit, cancellationToken);
        }
      }
    }

    private void CompileStatements<T>(ImmutableArray<T> statements)
      where T : Statement
    {
      if (statements.Length > 0)
      {
        foreach (var statement in statements)
        {
          CompileStatement(statement);
        }

        builder.AppendLine();
      }
    }

    [SuppressMessage("ReSharper", "CognitiveComplexity")]
    private void CompileStatement(Statement statement)
    {
      switch (statement)
      {
        case Include:
          // no-op (included in preamble)
          break;

        case Comment(var text):
          builder.AppendComment(text);
          break;

        case UniformDeclaration(var type, var name):
          builder.AppendUniformDeclaration(ConvertPrecision(type.Precision), ConvertType(type), name);
          break;

        case VaryingDeclaration(var type, var name):
          builder.AppendVaryingDeclaration(ConvertPrecision(type.Precision), ConvertType(type), name);
          break;

        case ConstantDeclaration(var type, var name, var value):
          builder.AppendConstantDeclaration(ConvertPrecision(type.Precision), ConvertType(type), name, CompileExpression(value));
          break;

        case Assignment(var variable, var value):
          builder.AppendAssignment(variable, CompileExpression(value));
          break;

        // TODO: convert this into something GLSL specific
        case IntrinsicAssignment(var intrinsicType, var value):
          builder.AppendAssignment(ConvertIntrinsic(intrinsicType), CompileExpression(value));
          break;

        case StageDeclaration(var kind) declaration:
        {
          using var function = builder.AppendFunctionDeclaration(
            precision: null,
            returnType: "void",
            name: "main",
            parameters: Enumerable.Empty<string>()
          );

          foreach (var functionStatement in declaration.Statements)
          {
            CompileStatement(functionStatement);
          }

          break;
        }

        case FunctionDeclaration(var returnType, var name) declaration:
        {
          using var function = builder.AppendFunctionDeclaration(
            precision: ConvertPrecision(returnType.Precision),
            returnType: ConvertType(returnType),
            name: name,
            parameters: declaration.Parameters.Select(CompileExpression)
          );

          foreach (var functionStatement in declaration.Statements)
          {
            CompileStatement(functionStatement);
          }

          break;
        }

        default:
          throw new InvalidOperationException($"An unexpected instruction was encountered: {statement}");
      }
    }

    private static string CompileExpression(Expression expression)
    {
      switch (expression)
      {
        case Expression.Constant(var value):
          return value.ToString() ?? string.Empty;

        case Expression.Variadic(var values):
          return $"{string.Join(", ", values.Select(CompileExpression))}";

        case Expression.Parameter(var type, var name):
          return $"{ConvertType(type)} {name}";

        case Expression.Constructor({ Cardinality: > 0 } type, var value):
          return $"{ConvertType(type)}({CompileExpression(value)})";

        case Expression.Constructor(_, var value):
          return CompileExpression(value);

        case Expression.Binary(var @operator, var left, var right):
          return $"{CompileExpression(left)} {ConvertOperator(@operator)} {CompileExpression(right)}";

        case Expression.Unary(var @operator, var value):
          return $"{ConvertOperator(@operator)}{CompileExpression(value)}";

        default:
          throw new InvalidOperationException($"An unexpected expression was encountered: {expression}");
      }
    }

    private static string? ConvertPrecision(Precision? precision) => precision switch
    {
      null             => null,
      Precision.Low    => "lowp",
      Precision.Medium => "mediump",
      Precision.High   => "highp",

      _ => throw new ArgumentOutOfRangeException(nameof(precision), precision, null),
    };

    private static string ConvertIntrinsic(IntrinsicType type) => type switch
    {
      IntrinsicType.Position => "POSITION",
      IntrinsicType.Color    => "COLOR",

      _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };

    private static string ConvertType(Primitive type) => type.Type switch
    {
      PrimitiveType.Void                                  => "void",
      PrimitiveType.Bool when !type.Cardinality.HasValue  => "bool",
      PrimitiveType.Bool when type.Cardinality.HasValue   => $"bvec{type.Cardinality}",
      PrimitiveType.Int when !type.Cardinality.HasValue   => "int",
      PrimitiveType.Int when type.Cardinality.HasValue    => $"ivec{type.Cardinality}",
      PrimitiveType.UInt when !type.Cardinality.HasValue  => "uint",
      PrimitiveType.UInt when type.Cardinality.HasValue   => $"uvec{type.Cardinality}",
      PrimitiveType.Float when !type.Cardinality.HasValue => "float",
      PrimitiveType.Float when type.Cardinality.HasValue  => $"vec{type.Cardinality}",
      PrimitiveType.Matrix when type.Cardinality.HasValue => $"mat{type.Cardinality}",
      PrimitiveType.Sampler                               => "sampler2D",

      _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };

    private static string ConvertOperator(BinaryOperator type) => type switch
    {
      BinaryOperator.Add      => "+",
      BinaryOperator.Subtract => "-",
      BinaryOperator.Multiply => "*",
      BinaryOperator.Divide   => "/",

      _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };

    private static string ConvertOperator(UnaryOperator type) => type switch
    {
      UnaryOperator.Negate => "-",

      _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
  }

  /// <summary>A utility for building shader programs from raw source text.</summary>
  private sealed record ShaderCodeBuilder(StringBuilder StringBuilder) : IDisposable
  {
    private int IndentLevel { get; set; } = 0;

    private StringBuilder StartLine() => StringBuilder.AppendIndent(IndentLevel);

    public void AppendLine()           => StringBuilder.AppendLine();
    public void AppendLine(string raw) => StringBuilder.AppendLine(raw);

    public void AppendComment(string text)
      => StartLine().AppendSpaced("/*").AppendSpaced(text).AppendLine("*/");

    public void AppendUniformDeclaration(string? precision, string type, string name)
      => StartLine().AppendSpaced("uniform").AppendSpaced(precision).AppendSpaced(type).Append(name).AppendLine(";");

    public void AppendVaryingDeclaration(string? precision, string type, string name)
      => StartLine().AppendSpaced("varying").AppendSpaced(precision).AppendSpaced(type).Append(name).AppendLine(";");

    public void AppendConstantDeclaration(string? precision, string type, string name, string value)
      => StartLine().AppendSpaced("const").AppendSpaced(precision).AppendSpaced(type).Append(name).Append(" = ").Append(value).AppendLine(";");

    public void AppendAssignment(string name, string value)
      => StartLine().Append(name).Append(" = ").Append(value).AppendLine(";");

    public ShaderCodeBuilder AppendFunctionDeclaration(string? precision, string returnType, string name, IEnumerable<string> parameters)
    {
      StartLine().AppendSpaced(precision).AppendSpaced(returnType).Append(name).Append('(').Append(string.Join(", ", parameters)).Append(") ").AppendLine(" {");

      IndentLevel += 1;
      return this;
    }

    public void Dispose()
    {
      IndentLevel -= 1;

      StartLine().AppendLine("}");
    }
  }
}
