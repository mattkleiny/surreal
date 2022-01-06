namespace Surreal.Graphics.Shaders;

public class SimpleShaderParserTests
{
  [Test, Benchmark(Milliseconds = 0.5f)]
  public async Task it_should_parse_a_simple_program()
  {
    IShaderParser language = new SimpleShaderParser();

    var metadata = await language.ParseShaderAsync(
      name: "test.shader",
      sourceCode: @"
      // A simple sprite shader, for testing purposes.
      // This description should be attached up-front.

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
