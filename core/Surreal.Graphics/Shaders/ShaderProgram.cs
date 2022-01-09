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

  public override async ValueTask<ShaderProgram> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken = default)
  {
    var program = await LoadShaderAsync(context.Path, cancellationToken);

    if (hotReloading && context.Path.GetFileSystem().SupportsWatcher)
    {
      var watcher = context.Path.Watch();

      return new HotLoadingShaderProgram(this, watcher, program);
    }

    return program;
  }

  private async ValueTask<ShaderProgram> LoadShaderAsync(VirtualPath path, CancellationToken cancellationToken = default)
  {
    await using var stream = await path.OpenInputStreamAsync();

    var parsed   = await parser.ParseShaderAsync(path.ToString(), stream, encoding, cancellationToken);
    var compiled = await device.ShaderCompiler.CompileAsync(parsed);

    return device.CreateShaderProgram(compiled);
  }

  /// <summary>A <see cref="ShaderProgram"/> that supports hot reloading from the file system.</summary>
  private sealed class HotLoadingShaderProgram : ShaderProgram
  {
    private readonly ShaderProgramLoader loader;
    private readonly IPathWatcher        watcher;
    private          ShaderProgram?      program;

    public HotLoadingShaderProgram(ShaderProgramLoader loader, IPathWatcher watcher, ShaderProgram program)
    {
      this.loader  = loader;
      this.watcher = watcher;
      this.program = program;

      watcher.Created  += OnShaderModified;
      watcher.Modified += OnShaderModified;
      watcher.Deleted  += OnShaderDeleted;
    }

    public override void Bind(VertexDescriptorSet descriptors)
    {
      program?.Bind(descriptors);
    }

    public override void SetUniform(string name, int scalar)            => program?.SetUniform(name, scalar);
    public override void SetUniform(string name, float scalar)          => program?.SetUniform(name, scalar);
    public override void SetUniform(string name, Vector2I point)        => program?.SetUniform(name, point);
    public override void SetUniform(string name, Vector3I point)        => program?.SetUniform(name, point);
    public override void SetUniform(string name, Vector2 vector)        => program?.SetUniform(name, vector);
    public override void SetUniform(string name, Vector3 vector)        => program?.SetUniform(name, vector);
    public override void SetUniform(string name, Vector4 vector)        => program?.SetUniform(name, vector);
    public override void SetUniform(string name, Quaternion quaternion) => program?.SetUniform(name, quaternion);
    public override void SetUniform(string name, in Matrix3x2 matrix)   => program?.SetUniform(name, in matrix);
    public override void SetUniform(string name, in Matrix4x4 matrix)   => program?.SetUniform(name, in matrix);

    private void OnShaderModified(VirtualPath path)
    {
      // TODO: relink the shader
    }

    private void OnShaderDeleted(VirtualPath path)
    {
      // TODO: delete the shader
    }

    protected override void Dispose(bool managed)
    {
      if (managed)
      {
        watcher.Dispose();
        program?.Dispose();
      }

      base.Dispose(managed);
    }
  }
}
