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
      return new Shader(type, await path.ReadAllBytesAsync());
    }

    public ShaderType   Type  { get; }
    public Memory<byte> Bytes { get; }

    public Shader(ShaderType type, Memory<byte> bytes) {
      Type  = type;
      Bytes = bytes;
    }
  }
}