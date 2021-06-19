using System;
using System.Threading.Tasks;
using Surreal.IO;

namespace Surreal.Graphics.Materials
{
  public enum ShaderType : byte
  {
    Vertex   = 0,
    Fragment = 1,
    Geometry = 2,
    Compute  = 3,
  }

  public sealed record Shader(ShaderType Type, Memory<byte> Bytecode)
  {
    public static async Task<Shader> LoadAsync(ShaderType type, Path path)
    {
      return new(type, await path.ReadAllBytesAsync());
    }
  }
}