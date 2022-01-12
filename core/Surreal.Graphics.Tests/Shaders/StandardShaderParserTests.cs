using Surreal.Graphics.Shaders.Languages;
using Surreal.IO;

namespace Surreal.Graphics.Shaders;

public class StandardShaderParserTests
{
  [Test]
  [TestCase("Assets/shaders/test01.shader")]
  public async Task it_should_parse_shader_programs(VirtualPath path)
  {
    var parser   = new StandardShaderParser();
    var metadata = await parser.ParseShaderAsync(path);

    Assert.IsNotNull(metadata);
    Assert.IsTrue(metadata.CompilationUnit.ShaderType is { Type: "sprite" });
    Assert.AreEqual(2, metadata.CompilationUnit.Includes.Length);
    Assert.AreEqual(2, metadata.CompilationUnit.Uniforms.Length);
    Assert.AreEqual(1, metadata.CompilationUnit.Varyings.Length);
    Assert.AreEqual(2, metadata.CompilationUnit.Stages.Length);
  }
}
