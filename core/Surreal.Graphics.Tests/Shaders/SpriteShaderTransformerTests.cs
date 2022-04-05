using Surreal.Graphics.Shaders.Transformers;
using static Surreal.Graphics.Shaders.ShaderSyntaxTree;

namespace Surreal.Graphics.Shaders;

public class SpriteShaderTransformerTests
{
  [Test]
  public async Task it_should_add_default_include_for_shader_base_utilities()
  {
    var transformer = new SpriteShaderTransformer();
    var compilationUnit = await transformer.TransformAsync(new CompilationUnit());

    compilationUnit.Includes.Count.Should().Be(1);
  }

  [Test]
  public async Task it_should_add_default_vertex_stage_if_none_exists()
  {
    var transformer = new SpriteShaderTransformer();
    var compilationUnit = await transformer.TransformAsync(new CompilationUnit());

    compilationUnit.Stages.Where(_ => _.Kind == ShaderKind.Vertex).Should().HaveCount(1);
  }

  [Test]
  public async Task it_should_add_default_fragment_stage_if_none_exists()
  {
    var transformer = new SpriteShaderTransformer();
    var compilationUnit = await transformer.TransformAsync(new CompilationUnit());

    compilationUnit.Stages.Where(_ => _.Kind == ShaderKind.Fragment).Should().HaveCount(1);
  }
}
