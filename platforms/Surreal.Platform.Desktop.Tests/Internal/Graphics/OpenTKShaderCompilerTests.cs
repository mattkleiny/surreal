using Surreal.Graphics.Shaders;
using Surreal.Graphics.Shaders.Languages;
using Surreal.IO;

namespace Surreal.Internal.Graphics;

public class OpenTKShaderCompilerTests
{
  [Test]
  [TestCase("Assets/shaders/test01.shader")]
  public async Task it_should_compile_shader_programs(VirtualPath path)
  {
    var parser   = new StandardShaderParser();
    var compiler = new OpenTKShaderCompiler();
    var context  = new ShaderCompilerEnvironment(parser);

    var declaration = await parser.ParseShaderAsync(path);
    var compiled    = await compiler.CompileAsync(context, declaration);

    Assert.IsNotNull(compiled);
  }
}
