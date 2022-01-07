namespace Surreal.Graphics.Shaders;

public class RoslynShaderParserTests
{
  [Test, Benchmark(ThresholdMs = 0.2f), Ignore("Not yet implemented")]
  public async Task it_should_parse_a_simple_program()
  {
    IShaderParser language = new RoslynShaderParser();

    var metadata = await language.ParseShaderAsync(
      path: "test.shader",
      sourceCode: @"
      // A simple sprite shader, for testing purposes.
      // This description should be attached up-front.

      namespace Surreal.Graphics.Shaders;

      [SpriteShader]
      public static class TestShader
      {
        [VertexStage]
        public static void Vertex()
        {
        }

        [FragmentStage]
        public static void Fragment([Color] out Color)
        {
          Color = Color.White;
        }
      }
    ");

    Assert.IsNotNull(metadata);
  }
}
