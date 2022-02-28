using Surreal.IO;

namespace Surreal.Graphics.Shaders;

public class StandardShaderParserTests
{
  [Test]
  [TestCase("Assets/shaders/test01.shade")]
  [TestCase("Assets/shaders/test02.shade")]
  public async Task it_should_parse_shader_programs(VirtualPath path)
  {
    var parser      = new StandardShaderParser();
    var declaration = await parser.ParseAsync(path);

    Assert.IsNotNull(declaration);
    Assert.IsTrue(declaration.CompilationUnit.ShaderType is { Type: "sprite" });
    Assert.AreEqual(1, declaration.CompilationUnit.Includes.Count);
    Assert.AreEqual(2, declaration.CompilationUnit.Stages.Length);
  }
}
