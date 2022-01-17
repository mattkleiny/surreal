using Surreal.IO;

namespace Surreal.Graphics.Shaders;

public class StandardShaderParserTests
{
  [Test]
  [TestCase("Assets/shaders/test01.shade")]
  public async Task it_should_parse_shader_programs(VirtualPath path)
  {
    var parser      = new StandardShaderParser();
    var declaration = await parser.ParseShaderAsync(path);

    Assert.IsNotNull(declaration);
    Assert.IsTrue(declaration.CompilationUnit.ShaderType is { Type: "sprite" });
    Assert.AreEqual(1, declaration.CompilationUnit.Includes.Count);
    Assert.AreEqual(4, declaration.CompilationUnit.Uniforms.Length);
    Assert.AreEqual(1, declaration.CompilationUnit.Varyings.Length);
    Assert.AreEqual(2, declaration.CompilationUnit.Stages.Length);
  }
}
