using System.Collections.Generic;
using Surreal.Graphics.Materials.Preprocessor.Backends;

namespace Surreal.Graphics.Materials.Preprocessor {
  internal sealed class ShadyCompiler {
    private readonly ICompilerBackend backend;

    public ShadyCompiler(ICompilerBackend backend) {
      this.backend = backend;
    }

    public Shader Compile(ShadyProgramType programType, ShaderType shaderType, IEnumerable<ShadyStatement> statements) {
      return new Shader(shaderType, backend.Compile(programType, shaderType, statements));
    }
  }
}