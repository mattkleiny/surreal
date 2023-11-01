namespace Surreal.Graphics.Materials;

public class ShaderCompilerTests
{
  [Test]
  public async Task it_should_compile_a_simple_file()
  {
    var parser = new ShaderParser();
    var shader = await parser.ParseAsync("Assets/External/shaders/test01.shade");
    var compiler = ShaderCompiler.Glsl;

    var kernels = compiler.Compile(shader);

    Assert.AreEqual(2, kernels.Length);
    Assert.AreEqual(ShaderType.VertexShader, kernels[0].Type);
    Assert.AreEqual(ShaderType.FragmentShader, kernels[1].Type);
  }
}
