using System.Threading.Tasks;
using Surreal.IO;

namespace Surreal.Graphics.Materials
{
  public readonly struct Shader
  {
    public static async Task<Shader> LoadAsync(ShaderType type, Path path)
    {
      var raw = await path.ReadAllBytesAsync();

      return new Shader(type, raw);
    }

    public ShaderType Type { get; }
    public byte[]     Raw  { get; }

    public Shader(ShaderType type, byte[] raw)
    {
      Type = type;
      Raw  = raw;
    }
  }
}