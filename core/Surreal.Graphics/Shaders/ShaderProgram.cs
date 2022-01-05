using Surreal.Assets;
using Surreal.Graphics.Meshes;
using Surreal.IO;
using Surreal.Mathematics;

namespace Surreal.Graphics.Shaders;

/// <summary>A low-level shader program on the GPU.</summary>
public abstract class ShaderProgram : GraphicsResource
{
  public abstract void Bind(VertexDescriptorSet descriptors);

  public abstract void SetUniform(string name, int scalar);
  public abstract void SetUniform(string name, float scalar);
  public abstract void SetUniform(string name, Vector2I point);
  public abstract void SetUniform(string name, Vector3I point);
  public abstract void SetUniform(string name, Vector2 vector);
  public abstract void SetUniform(string name, Vector3 vector);
  public abstract void SetUniform(string name, Vector4 vector);
  public abstract void SetUniform(string name, Quaternion quaternion);
  public abstract void SetUniform(string name, in Matrix3x2 matrix);
  public abstract void SetUniform(string name, in Matrix4x4 matrix);
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="ShaderProgram"/>s.</summary>
public sealed class ShaderProgramLoader : AssetLoader<ShaderProgram>
{
  private readonly IGraphicsDevice device;
  private readonly IShaderParser   parser;
  private readonly Encoding        encoding;
  private readonly bool            hotReloading;

  public ShaderProgramLoader(IGraphicsDevice device, IShaderParser parser, bool hotReloading)
    : this(device, parser, hotReloading, Encoding.UTF8)
  {
  }

  public ShaderProgramLoader(IGraphicsDevice device, IShaderParser parser, bool hotReloading, Encoding encoding)
  {
    this.device       = device;
    this.parser       = parser;
    this.encoding     = encoding;
    this.hotReloading = hotReloading;
  }

  public override async Task<ShaderProgram> LoadAsync(VirtualPath path, IAssetContext context, CancellationToken cancellationToken = default)
  {
    if (hotReloading)
    {
      // TODO: implement hot reloading with a file watcher
    }

    await using var stream = await path.OpenInputStreamAsync();

    var parsed   = await parser.ParseShaderAsync(path.ToString(), stream, encoding, cancellationToken);
    var compiled = await device.ShaderCompiler.CompileAsync(parsed);

    return device.CreateShaderProgram(compiled);
  }
}
