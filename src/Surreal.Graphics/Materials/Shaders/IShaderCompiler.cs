using System.Collections.Generic;
using static Surreal.Graphics.Materials.Shaders.ShaderStatement;

namespace Surreal.Graphics.Materials.Shaders {
  public interface IShaderCompiler {
    Shader Compile(
        ShaderProgramType programType,
        ShaderType shaderType,
        IEnumerable<UniformDeclarationStatement> uniforms,
        IEnumerable<FunctionDeclarationStatement> functions
    );
  }
}