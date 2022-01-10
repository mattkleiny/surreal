using Surreal.IO;

namespace Surreal.Graphics.Shaders;

public class StandardShaderParserTests
{
  [Test]
  [TestCase("Assets/shaders/test01.shader")]
  public async Task it_should_parse_shader_programs(VirtualPath path)
  {
    await using var stream   = await path.OpenInputStreamAsync();
    var             language = new StandardShaderParser();

    var metadata = await language.ParseShaderAsync(path.ToString(), stream);

    Assert.IsNotNull(metadata);
  }
}
