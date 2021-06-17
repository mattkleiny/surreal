using System;
using System.Threading.Tasks;
using Surreal.IO;

namespace Surreal.Graphics.Materials.Shaders {
  public enum ShaderType : byte {
    Vertex   = 0,
    Fragment = 1,
    Geometry = 2,
    Compute  = 3,
  }

  public readonly struct Shader {
    public static async Task<Shader> LoadAsync(ShaderType type, Path path) {
      return new(type, await path.ReadAllBytesAsync());
    }

    public ShaderType   Type     { get; }
    public Memory<byte> Bytecode { get; }

    public Shader(ShaderType type, Memory<byte> bytecode) {
      Type     = type;
      Bytecode = bytecode;
    }
  }
}