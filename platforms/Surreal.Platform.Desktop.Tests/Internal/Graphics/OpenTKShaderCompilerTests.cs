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
    var context  = new ShaderCompilerContext(parser);

    var declaration = await parser.ParseShaderAsync(path);
    var compiled    = await compiler.CompileAsync(context, declaration);

    Assert.IsNotNull(compiled);
  }

  private sealed class ShaderCompilerContext : IShaderCompilerContext
  {
    private readonly IShaderParser parser;

    public ShaderCompilerContext(IShaderParser parser)
    {
      this.parser = parser;
    }

    public ValueTask<ShaderDeclaration> ExpandShaderAsync(VirtualPath path, CancellationToken cancellationToken = default)
    {
      return parser.ParseShaderAsync(path, cancellationToken);
    }
  }
}
