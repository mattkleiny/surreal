using Surreal.IO;

namespace Surreal.Graphics.Shaders.Parsing;

public class ShaderParserTests
{
  [Test, Ignore("Not yet implemented")]
  [TestCase("Assets/shaders/test01.shade")]
  [TestCase("Assets/shaders/test02.shade")]
  [TestCase("resx://Surreal.Graphics/Resources/shaders/common.shade")]
  [TestCase("resx://Surreal.Graphics/Resources/shaders/sprite.shade")]
  public async Task it_should_parse_shader_programs(VirtualPath path)
  {
    var parser = new ShaderParser();
    var declaration = await parser.ParseAsync(path);

    declaration.Should().NotBeNull();
  }
}
