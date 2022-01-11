using Surreal.IO;

namespace Surreal.Graphics.Shaders;

public class StandardShaderParserTests
{
  [Test]
  [TestCase("Assets/shaders/test01.shader")]
  public async Task it_should_parse_shader_programs(VirtualPath path)
  {
    var parser   = new StandardShaderParser();
    var metadata = await parser.ParseShaderAsync(path);

    Assert.IsNotNull(metadata);
  }
}
