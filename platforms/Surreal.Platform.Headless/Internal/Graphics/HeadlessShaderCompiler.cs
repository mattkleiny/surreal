using Surreal.Graphics.Shaders;
using Surreal.Internal.Graphics.Resources;

namespace Surreal.Internal.Graphics;

internal sealed class HeadlessShaderCompiler : IShaderCompiler
{
  public ICompiledShaderProgram Compile(ShaderDeclaration declaration)
  {
    return new HeadlessCompiledShader(declaration.Path);
  }
}
