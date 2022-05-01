﻿namespace Surreal.Graphics.Shaders;

public class ShadeLanguageTests
{
  [Test]
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
