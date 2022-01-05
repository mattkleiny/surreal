using Surreal.Graphics.Shaders;

namespace Surreal.Internal.Graphics;

internal sealed class HeadlessShaderCompiler : IShaderCompiler
{
  public Task<ICompiledShader> CompileAsync(IParsedShader shader)
  {
    throw new NotImplementedException();
  }
}
