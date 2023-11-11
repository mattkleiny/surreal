namespace Surreal.Graphics.Materials;

[Ignore("Not yet implemented")]
public class ShaderParserTests
{
  [Test]
  public async Task it_should_parse_a_simple_file()
  {
    var parser = new ShaderParser();
    var shader = await parser.ParseAsync("Assets/External/shaders/test01.shade");

    shader.Should().NotBeNull();
  }
}
