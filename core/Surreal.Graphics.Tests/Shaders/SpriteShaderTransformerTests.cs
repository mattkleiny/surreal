using Surreal.Graphics.Shaders.Transformers;

namespace Surreal.Graphics.Shaders;

public class SpriteShaderTransformerTests
{
  [Test]
  public void it_should_add_default_include_for_shader_base_utilities()
  {
    var transformer = new SpriteShaderTransformer();
    var compilationUnit = transformer.Transform(new ShaderCompilationUnit());

    compilationUnit.Includes.Count.Should().Be(1);
  }

  [Test]
  public void it_should_add_default_vertex_stage_if_none_exists()
  {
    var transformer = new SpriteShaderTransformer();
    var compilationUnit = transformer.Transform(new ShaderCompilationUnit());

    compilationUnit.Stages.Where(_ => _.Kind == ShaderKind.Vertex).Should().HaveCount(1);
  }

  [Test]
  public void it_should_add_default_fragment_stage_if_none_exists()
  {
    var transformer = new SpriteShaderTransformer();
    var compilationUnit = transformer.Transform(new ShaderCompilationUnit());

    compilationUnit.Stages.Where(_ => _.Kind == ShaderKind.Fragment).Should().HaveCount(1);
  }
}
