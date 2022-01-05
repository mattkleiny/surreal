using Surreal.Graphics.Shaders;
using Surreal.Internal.Graphics.Resources;

namespace Surreal.Internal.Graphics;

internal sealed class HeadlessShaderCompiler : IShaderCompiler
{
  public Task<ICompiledShader> CompileAsync(IParsedShader shader)
  {
    return Task.FromResult<ICompiledShader>(new HeadlessShaderSet());
  }
}
