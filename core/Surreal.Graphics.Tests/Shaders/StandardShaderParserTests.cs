namespace Surreal.Graphics.Shaders;

public class StandardShaderParserTests
{
  [Test, Benchmark(ThresholdMs = 0.2f)]
  public async Task it_should_parse_a_simple_program()
  {
    IShaderParser language = new StandardShaderParser();

    var metadata = await language.ParseShaderAsync(
      path: "test.shader",
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
