namespace Surreal.Graphics.Shaders.Parsing;

public class ShadeLanguageTests
{
  [Test, Ignore("Not yet implemented")]
  public void it_should_parse_simple_expressions()
  {
    const string code = @"
      uniform sampler2d texture;
      uniform lowp vec3 tint;
      varying highp vec4 color;
    ";

    var result = ShadeLanguage.Parse(code);

    result.Should().NotBeNull();
  }
}
