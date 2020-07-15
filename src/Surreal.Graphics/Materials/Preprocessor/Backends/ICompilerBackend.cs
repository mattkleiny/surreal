using System.Collections.Generic;

namespace Surreal.Graphics.Materials.Preprocessor.Backends {
  internal interface ICompilerBackend {
    byte[] Compile(ShadyProgramType programType, ShaderType shaderType, IEnumerable<ShadyStatement> statements);
  }
}