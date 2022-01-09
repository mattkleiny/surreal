using OpenTK.Graphics.OpenGL;
using Surreal.Graphics.Shaders;
using Surreal.Internal.Graphics.Resources;
using Surreal.Text;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree.Statement;
using PrimitiveType = Surreal.Graphics.Shaders.PrimitiveType;

namespace Surreal.Internal.Graphics;

internal sealed class OpenTKShaderCompiler : IShaderCompiler
{
  private readonly string version;

  public OpenTKShaderCompiler(string version = "330")
  {
    this.version = version;
  }

  public ValueTask<ICompiledShaderProgram> CompileAsync(ShaderProgramDeclaration declaration)
  {
    var shaders = ImmutableArray.CreateBuilder<OpenTKShader>(declaration.CompilationUnit.Stages.Length);

    foreach (var stage in declaration.CompilationUnit.Stages)
    {
      var shaderType = ConvertShaderKind(stage.Kind);
      var sourceCode = BuildSourceCode(declaration, declaration.CompilationUnit, stage);

      shaders.Add(new OpenTKShader(shaderType, sourceCode));
    }

    return ValueTask.FromResult<ICompiledShaderProgram>(new OpenTKShaderSet(declaration.Path, shaders.MoveToImmutable()));
  }

  private string BuildSourceCode(ShaderProgramDeclaration declaration, CompilationUnit compilationUnit, StageDeclaration stage)
  {
    var stringBuilder = new StringBuilder();
    var codeBuilder   = new ShaderCodeBuilder(stringBuilder);

    // compile preamble and include other modules
    CompilePreamble(codeBuilder, declaration);

    codeBuilder.AppendBlankLine();

    if (compilationUnit.Includes.Length > 0)
    {
      CompileIncludes(codeBuilder, compilationUnit.Includes);

      codeBuilder.AppendBlankLine();
    }

    // compile globals
    if (compilationUnit.Uniforms.Length > 0)
    {
      foreach (var uniform in compilationUnit.Uniforms)
      {
        CompileStatement(codeBuilder, uniform);
      }

      codeBuilder.AppendBlankLine();
    }

    if (compilationUnit.Varyings.Length > 0)
    {
      foreach (var varying in compilationUnit.Varyings)
      {
        CompileStatement(codeBuilder, varying);
      }

      codeBuilder.AppendBlankLine();
    }

    if (compilationUnit.Constants.Length > 0)
    {
      foreach (var constant in compilationUnit.Constants)
      {
        CompileStatement(codeBuilder, constant);
      }

      codeBuilder.AppendBlankLine();
    }

    if (compilationUnit.Functions.Length > 0)
    {
      foreach (var function in compilationUnit.Functions)
      {
        CompileStatement(codeBuilder, function);
      }
    }

    // compile stage local
    CompileStatement(codeBuilder, stage);

    return stringBuilder.ToString();
  }

  private void CompilePreamble(ShaderCodeBuilder builder, ShaderProgramDeclaration declaration)
  {
    builder.AppendComment(declaration.Path);
    builder.AppendBlankLine();
    builder.AppendLine($"#version {version}");
  }

  private static void CompileIncludes(ShaderCodeBuilder builder, IEnumerable<Include> includes)
  {
    foreach (var include in includes)
    {
      // TODO: actually insert the included module
      builder.AppendLine($"#include {include.Module}");
    }
  }

  private static void CompileStatement(ShaderCodeBuilder builder, Statement statement)
  {
    switch (statement)
    {
      case Include:
        // no-op (included in preamble)
        break;

      case BlankLine:
        builder.AppendBlankLine();
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
          name: kind.ToString().ToLower(),
          parameters: declaration.Parameters.Select(CompileExpression)
        );

        foreach (var functionStatement in declaration.Statements)
        {
          CompileStatement(function, functionStatement);
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
          CompileStatement(function, functionStatement);
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
    BinaryOperator.Add => "+",
    BinaryOperator.Sub => "-",
    BinaryOperator.Mul => "*",
    BinaryOperator.Div => "/",

    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
  };

  private static string ConvertOperator(UnaryOperator type) => type switch
  {
    UnaryOperator.Negate => "-",

    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
  };

  private static ShaderType ConvertShaderKind(ShaderKind kind) => kind switch
  {
    ShaderKind.Vertex   => ShaderType.VertexShader,
    ShaderKind.Geometry => ShaderType.GeometryShader,
    ShaderKind.Fragment => ShaderType.FragmentShader,

    _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
  };

  /// <summary>A utility for building shader programs from raw source text.</summary>
  private sealed record ShaderCodeBuilder(StringBuilder Builder, int IndentLevel = 0) : IDisposable
  {
    public void AppendLine(string raw) => Builder.AppendLine(raw);
    public void AppendBlankLine()      => Builder.AppendLine();

    public void AppendComment(string text)
    {
      Builder
        .AppendIndent(IndentLevel)
        .Append("/* ")
        .Append(text)
        .AppendLine(" */");
    }

    public void AppendUniformDeclaration(string? precision, string type, string name)
    {
      Builder
        .AppendIndent(IndentLevel)
        .Append("uniform ")
        .Append(precision)
        .Append(precision != null ? ' ' : null)
        .Append(type)
        .Append(' ')
        .Append(name)
        .AppendLine(";");
    }

    public void AppendVaryingDeclaration(string? precision, string type, string name)
    {
      Builder
        .AppendIndent(IndentLevel)
        .Append("varying ")
        .Append(precision)
        .Append(precision != null ? ' ' : null)
        .Append(type)
        .Append(' ')
        .Append(name)
        .AppendLine(";");
    }

    public void AppendConstantDeclaration(string? precision, string type, string name, string value)
    {
      Builder
        .AppendIndent(IndentLevel)
        .Append("const ")
        .Append(precision)
        .Append(precision != null ? ' ' : null)
        .Append(type)
        .Append(' ')
        .Append(name)
        .Append(" = ")
        .Append(value)
        .AppendLine(";");
    }

    public void AppendAssignment(string name, string value)
    {
      Builder
        .AppendIndent(IndentLevel)
        .Append(name)
        .Append(" = ")
        .Append(value)
        .AppendLine(";");
    }

    public ShaderCodeBuilder AppendFunctionDeclaration(string? precision, string returnType, string name, IEnumerable<string> parameters)
    {
      Builder
        .AppendIndent(IndentLevel)
        .Append(precision)
        .Append(precision != null ? ' ' : null)
        .Append(returnType)
        .Append(' ')
        .Append(name)
        .Append("(")
        .Append(string.Join(", ", parameters))
        .Append(")")
        .AppendLine(" {");

      return this with { IndentLevel = IndentLevel + 1 };
    }

    public void Dispose()
    {
      Builder
        .AppendIndent(IndentLevel - 1)
        .AppendLine("}");
    }
  }
}
