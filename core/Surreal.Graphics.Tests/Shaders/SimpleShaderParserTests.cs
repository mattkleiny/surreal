﻿using NUnit.Framework;

namespace Surreal.Graphics.Shaders;

public class SimpleShaderParserTests
{
  [Test]
  public async Task it_should_parse_a_simple_program()
  {
    IShaderParser language = new SimpleShaderParser();

    var metadata = await language.ParseShaderAsync(
      name: "test.shader",
      sourceCode: @"
      shader_type sprite;

      uniform vec3  _Position;
      uniform float _Intensity;

      varying vec3  _Color;

      void vertex()
      {
        _Color = vec3(1,1,1) * _Position * _Intensity;
      }

      void fragment()
      {
        COLOR = _Color;
      }
    ");

    Assert.IsNotNull(metadata);
  }
}
