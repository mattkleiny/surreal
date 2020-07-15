using System;
using System.Collections.Generic;

namespace Surreal.Graphics.Materials.Shady.Backends {
  internal sealed class GLSLCompilerBackend : ICompilerBackend {
    public byte[] Compile(
        ShadyProgramType programType,
        ShaderType shaderType,
        IEnumerable<ShadyStatement> statements) {
      throw new NotImplementedException();
    }
  }
}