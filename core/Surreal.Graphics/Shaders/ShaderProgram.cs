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
public sealed class ShaderLoader : AssetLoader<ShaderProgram>
{
  private readonly IGraphicsDevice          device;
  private readonly ImmutableHashSet<string> extensions;

  public ShaderLoader(IGraphicsDevice device, params string[] extensions)
    : this(device, extensions.AsEnumerable())
  {
  }

  public ShaderLoader(IGraphicsDevice device, IEnumerable<string> extensions)
  {
    this.device     = device;
    this.extensions = extensions.ToImmutableHashSet();
  }

  public override bool CanHandle(AssetLoaderContext context)
  {
    return base.CanHandle(context) && extensions.Contains(context.Path.Extension);
  }

  public override async ValueTask<ShaderProgram> LoadAsync(AssetLoaderContext context, ProgressToken progressToken = default)
  {
    // TODO: support hot reloading?

    var environment = new ShaderAssetEnvironment(context.Manager);

    var declaration = await context.Manager.LoadAssetAsync<ShaderDeclaration>(context.Path);
    var compiled    = await device.ShaderCompiler.CompileAsync(environment, declaration, progressToken.CancellationToken);

    return device.CreateShaderProgram(compiled);
  }

  /// <summary>A <see cref="IShaderCompilerEnvironment"/> implementation that delegates back to the asset system..</summary>
  private sealed class ShaderAssetEnvironment : IShaderCompilerEnvironment
  {
    private readonly IAssetManager manager;

    public ShaderAssetEnvironment(IAssetManager manager)
    {
      this.manager = manager;
    }

    public async ValueTask<ShaderDeclaration> ExpandShaderAsync(VirtualPath path, CancellationToken cancellationToken = default)
    {
      return await manager.LoadAssetAsync<ShaderDeclaration>(path);
    }
  }
}
