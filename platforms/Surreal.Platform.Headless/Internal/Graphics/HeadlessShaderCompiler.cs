using Surreal.Graphics.Shaders;
using Surreal.Internal.Graphics.Resources;

namespace Surreal.Internal.Graphics;

internal sealed class HeadlessShaderCompiler : IShaderCompiler
{
  public ValueTask<ICompiledShaderProgram> CompileAsync(
    IShaderCompilerContext context,
    ShaderProgramDeclaration declaration,
    CancellationToken cancellationToken = default
  )
  {
    return ValueTask.FromResult<ICompiledShaderProgram>(new HeadlessCompiledShader(declaration.Path));
  }
}
