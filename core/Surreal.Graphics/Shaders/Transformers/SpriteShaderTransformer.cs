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
  public bool CanTransform(ShaderDeclaration declaration)
  {
    return declaration.CompilationUnit.ShaderType is { Type: "sprite" };
  }

  public ValueTask<ShaderDeclaration> TransformAsync(ShaderDeclaration declaration, CancellationToken cancellationToken = default)
  {
    IncludeDefaultResources(ref declaration);

    AttachDefaultVertexStage(ref declaration);
    AttachDefaultFragmentStage(ref declaration);

    return ValueTask.FromResult(declaration);
  }

  private static void IncludeDefaultResources(ref ShaderDeclaration declaration)
  {
    declaration = declaration with
    {
      CompilationUnit = declaration.CompilationUnit with
      {
        Includes = declaration.CompilationUnit.Includes.Insert(0, new Include("resx://Surreal.Graphics/Resources/shaders/common.shade"))
      }
    };
  }

  private static void AttachDefaultVertexStage(ref ShaderDeclaration declaration)
  {
    foreach (var stage in declaration.CompilationUnit.Stages)
    {
      if (stage.Kind == ShaderKind.Vertex)
      {
        break;
      }
    }

    declaration = declaration with
    {
      CompilationUnit = declaration.CompilationUnit with
      {
        Stages = declaration.CompilationUnit.Stages.Add(new StageDeclaration(ShaderKind.Vertex)
        {
          Parameters = ImmutableArray.Create(
            new Parameter(new Primitive(PrimitiveType.Float, 3), "position"),
            new Parameter(new Primitive(PrimitiveType.Float, 4), "color")
          ),
          Statements = ImmutableArray.Create<Statement>(
            new Assignment("POSITION", new Symbol("position"))
          )
        })
      }
    };
  }

  private void AttachDefaultFragmentStage(ref ShaderDeclaration declaration)
  {
    foreach (var stage in declaration.CompilationUnit.Stages)
    {
      if (stage.Kind == ShaderKind.Vertex)
      {
        break;
      }
    }

    declaration = declaration with
    {
      CompilationUnit = declaration.CompilationUnit with
      {
        Stages = declaration.CompilationUnit.Stages.Add(new StageDeclaration(ShaderKind.Fragment)
        {
          Statements = ImmutableArray.Create<Statement>(
            new Assignment("COLOR", new Symbol("_Color"))
          )
        })
      }
    };
  }
}
