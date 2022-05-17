using Surreal.Graphics.Shaders.Parsing;
using Surreal.IO;

namespace Surreal.Internal.Graphics;

public class OpenTKShaderCompilerTests
{
  private static readonly Version Version = new(3, 3, 0);

  [Test, Ignore("Not yet implemented")]
  [TestCase("resx://Surreal.Graphics/Resources/shaders/common.shade")]
  [TestCase("resx://Surreal.Graphics/Resources/shaders/sprite.shade")]
  public async Task it_should_compile_shader_programs(VirtualPath path)
  {
    var parser = new ShaderParser();
    var compiler = new OpenTKShaderCompiler(Version);

    var declaration = await parser.ParseAsync(path);
    var shaderSet = compiler.CompileShader(declaration);

    shaderSet.Should().NotBeNull();

    foreach (var shader in shaderSet.Shaders)
    {
      Console.WriteLine(shader.Code);
    }
  }
}
