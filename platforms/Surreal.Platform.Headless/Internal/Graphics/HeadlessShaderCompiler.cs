using Surreal.Graphics.Shaders;
using Surreal.Internal.Graphics.Resources;

namespace Surreal.Internal.Graphics;

internal sealed class HeadlessShaderCompiler : IShaderCompiler
{
  public Task<ICompiledShaderProgram> CompileAsync(ShaderProgramDeclaration declaration)
  {
    var result = new HeadlessCompiledShader(declaration.FileName, declaration.Description);

    return Task.FromResult<ICompiledShaderProgram>(result);
  }
}
