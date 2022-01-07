using Surreal.Graphics.Shaders;
using Surreal.Internal.Graphics.Resources;

namespace Surreal.Internal.Graphics;

internal sealed class HeadlessShaderCompiler : IShaderCompiler
{
  public Task<ICompiledShaderProgram> CompileAsync(ShaderProgramDeclaration declaration)
  {
    return Task.FromResult<ICompiledShaderProgram>(new HeadlessCompiledShader(declaration.Path));
  }
}
