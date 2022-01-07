using OpenTK.Graphics.OpenGL;
using Surreal.Graphics.Shaders;
using Surreal.Internal.Graphics.Resources;
using Surreal.Internal.Graphics.Utilities;
using static Surreal.Graphics.Shaders.ShaderInstruction;
using PrimitiveType = Surreal.Graphics.Shaders.PrimitiveType;

namespace Surreal.Internal.Graphics;

internal sealed class OpenTKShaderCompiler : IShaderCompiler
{
  private readonly string version;

  public OpenTKShaderCompiler(string version = "330")
  {
    this.version = version;
  }

  public Task<ICompiledShaderProgram> CompileAsync(ShaderProgramDeclaration declaration)
  {
    var shaders = new OpenTKShader[declaration.Shaders.Length];

    for (var i = 0; i < declaration.Shaders.Length; i++)
    {
      var (kind, instructions) = declaration.Shaders[i];

      var sourceCode = BuildSourceCode(declaration, instructions);
      var shaderType = ConvertShaderKind(kind);

      shaders[i] = new OpenTKShader(sourceCode, shaderType);
    }

    var shaderSet = new OpenTKShaderSet(declaration.FileName, declaration.Description, shaders);

    return Task.FromResult<ICompiledShaderProgram>(shaderSet);
  }

  private string BuildSourceCode(ShaderProgramDeclaration declaration, CompilationUnit compilationUnit)
  {
    var builder = new ShaderCodeBuilder();

    CompilePreamble(builder, declaration);
    CompileIncludes(builder, compilationUnit.Statements.OfType<Statement.Include>());

    foreach (var statement in compilationUnit.Statements)
    {
      CompileStatement(builder, statement);
    }

    return builder.ToSourceCode();
  }

  private void CompilePreamble(IShaderCodeBuilderScope builder, ShaderProgramDeclaration declaration)
  {
    builder.AppendComment(declaration.FileName);
    builder.AppendComment(declaration.Description);
    builder.AppendBlankLine();
    builder.AppendLine($"#version {version}");
    builder.AppendBlankLine();
  }

  private static void CompileIncludes(IShaderCodeBuilderScope builder, IEnumerable<Statement.Include> includes)
  {
    foreach (var include in includes)
    {
      // TODO: actually insert the included module
      builder.AppendLine($"#include {include.Module}");
    }
  }

  private static void CompileStatement(IShaderCodeBuilderScope builder, Statement statement)
  {
    switch (statement)
    {
      case Statement.Include:
        // no-op (included in preamble)
        break;

      case Statement.BlankLine:
        builder.AppendBlankLine();
        break;

      case Statement.Comment(var text):
        builder.AppendComment(text);
        break;

      case Statement.UniformDeclaration(var type, var name):
        builder.AppendUniformDeclaration(ConvertPrecision(type.Precision), ConvertType(type), name);
        break;

      case Statement.VaryingDeclaration(var type, var name):
        builder.AppendVaryingDeclaration(ConvertPrecision(type.Precision), ConvertType(type), name);
        break;

      case Statement.ConstantDeclaration(var type, var name, var value):
        builder.AppendConstantDeclaration(ConvertPrecision(type.Precision), ConvertType(type), name, CompileExpression(value));
        break;

      case Statement.Assignment(var variable, var value):
        builder.AppendAssignment(variable, CompileExpression(value));
        break;

      // TODO: convert this into something GLSL specific
      case Statement.IntrinsicAssignment(var intrinsicType, var value):
        builder.AppendAssignment(ConvertIntrinsic(intrinsicType), CompileExpression(value));
        break;

      case Statement.FunctionDeclaration(var returnType, var name, var body):
      {
        using var function = builder.AppendFunctionDeclaration(
          precision: ConvertPrecision(returnType.Precision),
          returnType: ConvertType(returnType),
          name: name,
          parameters: body.OfType<Expression.Parameter>().Select(CompileExpression)
        );

        foreach (var functionStatement in body.OfType<Statement>())
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

      case Expression.Constructor(var type, var value) when type.Cardinality > 0:
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

    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
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

    _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
  };
}
