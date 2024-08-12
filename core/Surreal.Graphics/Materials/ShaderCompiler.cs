using Surreal.Collections.Slices;
using Surreal.Text;
using static Surreal.Graphics.Materials.ShaderSyntaxTree.Statement;

namespace Surreal.Graphics.Materials;

/// <summary>
/// A shader compilation backend.
/// </summary>
public abstract class ShaderCompiler
{
  /// <summary>
  /// The default <see cref="GlslShaderCompiler"/> .
  /// </summary>
  public static ShaderCompiler Glsl { get; } = new GlslShaderCompiler();

  /// <summary>
  /// Compiles a <see cref="ShaderDeclaration"/> into <see cref="ShaderKernel"/>s.
  /// </summary>
  public abstract ReadOnlySlice<ShaderKernel> Compile(ShaderDeclaration declaration);
}

/// <summary>
/// A <see cref="ShaderCompiler"/> for GLSL.
/// </summary>
internal sealed class GlslShaderCompiler : ShaderCompiler
{
  public override ReadOnlySlice<ShaderKernel> Compile(ShaderDeclaration declaration)
  {
    var compilationUnit = declaration.CompilationUnit;
    var kernels = new List<ShaderKernel>();

    foreach (var stage in compilationUnit.Stages)
    {
      kernels.Add(CompileStage(declaration, stage));
    }

    return kernels;
  }

  /// <summary>
  /// Compiles a single stage into a <see cref="ShaderKernel"/>.
  /// </summary>
  private static ShaderKernel CompileStage(ShaderDeclaration declaration, StageDeclaration stage)
  {
    var type = stage.Kind switch
    {
      ShaderKind.Vertex => ShaderType.VertexShader,
      ShaderKind.Fragment => ShaderType.FragmentShader,

      _ => throw new ArgumentOutOfRangeException()
    };

    var builder = new IndentedStringBuilder();

    builder.AppendLine($"// {stage.Kind} compiled from {declaration.Path}");
    builder.AppendLine($"// Compiled using {nameof(ShaderCompiler)} v{typeof(ShaderCompiler).Assembly.GetName().Version}");
    builder.AppendLine();
    builder.Append("#version 330 core");
    builder.AppendLine();

    foreach (var (primitive, name) in declaration.CompilationUnit.Uniforms)
    {
      builder.AppendLine($"uniform {ToGlsl(primitive)} {name};");
    }

    builder.AppendLine();

    foreach (var (primitive, name) in declaration.CompilationUnit.Varyings)
    {
      builder.AppendLine($"varying {ToGlsl(primitive)} {name};");
    }

    builder.AppendLine();

    foreach (var (primitive, name, value) in declaration.CompilationUnit.Constants)
    {
      builder.AppendLine($"const {ToGlsl(primitive)} {name} = {value};");
    }

    builder.AppendLine();

    builder.AppendLine("void main()");
    builder.AppendLine("{");
    builder.Indent();

    foreach (var statement in stage.Statements)
    {
      // TODO: implement me properly
      switch (statement)
      {
        case Comment comment:
          builder.AppendLine($"// {comment.Text}");
          break;
      }
    }

    builder.Dedent();
    builder.AppendLine("}");

    return new ShaderKernel(type, builder.ToStringBuilder());
  }

  /// <summary>
  /// Converts a <see cref="ShaderPrimitive"/> to a GLSL type.
  /// </summary>
  private static string ToGlsl(ShaderPrimitive primitive)
  {
    return primitive switch
    {
      { Type: ShaderPrimitiveType.Float, Cardinality: null } => "float",
      { Type: ShaderPrimitiveType.Float, Cardinality: 1 } => "float",
      { Type: ShaderPrimitiveType.Float, Cardinality: 2 } => "vec2",
      { Type: ShaderPrimitiveType.Float, Cardinality: 3 } => "vec3",
      { Type: ShaderPrimitiveType.Matrix, Cardinality: 2 } => "mat2",
      { Type: ShaderPrimitiveType.Matrix, Cardinality: 3 } => "mat3",
      { Type: ShaderPrimitiveType.Matrix, Cardinality: 4 } => "mat4",
      { Type: ShaderPrimitiveType.Sampler, Cardinality: 1 } => "sampler1D",
      { Type: ShaderPrimitiveType.Sampler, Cardinality: 2 } => "sampler2D",
      { Type: ShaderPrimitiveType.Sampler, Cardinality: 3 } => "sampler2D",

      _ => throw new ArgumentOutOfRangeException(nameof(primitive), primitive, null)
    };
  }
}
