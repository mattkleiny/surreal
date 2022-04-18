using static Surreal.Graphics.Shaders.ShaderSyntaxTree;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree.Expression;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree.Statement;

namespace Surreal.Graphics.Shaders.Transformers;

/// <summary>
/// A <see cref="IShaderTransformer"/> that embeds some sprite-specific code into the resultant shader, and
/// provides a reasonable default vertex program for 99% use cases.
/// </summary>
public sealed class SpriteShaderTransformer : IShaderTransformer
{
  public bool CanTransform(ShaderCompilationUnit compilationUnit)
  {
    return compilationUnit.ShaderType is { Type: "sprite" };
  }

  public ShaderCompilationUnit Transform(ShaderCompilationUnit compilationUnit)
  {
    IncludeDefaultResources(ref compilationUnit);

    AttachDefaultVertexStage(ref compilationUnit);
    AttachDefaultFragmentStage(ref compilationUnit);

    return compilationUnit;
  }

  private static void IncludeDefaultResources(ref ShaderCompilationUnit compilationUnit)
  {
    compilationUnit = compilationUnit with
    {
      Includes = compilationUnit.Includes.Add(new Include("resx://Surreal.Graphics/Resources/shaders/common.shade")),
    };
  }

  private static void AttachDefaultVertexStage(ref ShaderCompilationUnit compilationUnit)
  {
    if (compilationUnit.Stages.Any(_ => _.Kind == ShaderKind.Vertex))
    {
      return;
    }

    compilationUnit = compilationUnit with
    {
      Stages = compilationUnit.Stages.Add(new StageDeclaration(ShaderKind.Vertex)
      {
        Parameters = ImmutableArray.Create(
          new Parameter(new Primitive(PrimitiveType.Float, 3), "position"),
          new Parameter(new Primitive(PrimitiveType.Float, 4), "color")
        ),
        Statements = ImmutableArray.Create<Statement>(
          new Assignment("POSITION", new Symbol("position"))
        ),
      }),
    };
  }

  private static void AttachDefaultFragmentStage(ref ShaderCompilationUnit compilationUnit)
  {
    if (compilationUnit.Stages.Any(_ => _.Kind == ShaderKind.Fragment))
    {
      return;
    }

    compilationUnit = compilationUnit with
    {
      Stages = compilationUnit.Stages.Add(new StageDeclaration(ShaderKind.Fragment)
      {
        Statements = ImmutableArray.Create<Statement>(
          new Assignment("COLOR", new Symbol("_Color"))
        ),
      }),
    };
  }
}
