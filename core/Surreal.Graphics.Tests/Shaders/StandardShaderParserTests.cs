using Surreal.IO;

namespace Surreal.Graphics.Shaders;

public class StandardShaderParserTests
{
  [Test]
  [TestCase("Assets/shaders/test01.shade")]
  [TestCase("Assets/shaders/test02.shade")]
  public async Task it_should_parse_shader_programs(VirtualPath path)
  {
    var parser = new StandardShaderParser();
    var declaration = await parser.ParseAsync(path);

    declaration.Should().NotBeNull();
    declaration.CompilationUnit.ShaderType.Type.Should().Be("sprite");
    declaration.CompilationUnit.Includes.Count.Should().Be(1);
    declaration.CompilationUnit.Stages.Length.Should().Be(2);
  }
}
