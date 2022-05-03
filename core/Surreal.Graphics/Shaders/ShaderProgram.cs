using Surreal.Assets;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;

namespace Surreal.Graphics.Shaders;

/// <summary>Utilities for <see cref="ShaderProgram"/>s.</summary>
public static class ShaderProgramExtensions
{
  /// <summary>Loads the default <see cref="ShaderProgram"/> for sprites from Surreal.</summary>
  public static async Task<ShaderProgram> LoadDefaultSpriteShaderAsync(this IAssetManager manager)
  {
    return await manager.LoadAssetAsync<ShaderProgram>("resx://Surreal.Graphics/Resources/shaders/sprite.glsl");
  }
}

/// <summary>A low-level shader program on the GPU.</summary>
public sealed class ShaderProgram : GraphicsResource
{
  private readonly IGraphicsServer server;

  public ShaderProgram(IGraphicsServer server)
  {
    this.server = server;

    Handle = server.CreateShader();
  }

  public GraphicsHandle Handle { get; private set; }

  public void SetUniform(string name, int value)
    => server.SetShaderUniform(Handle, name, value);

  public void SetUniform(string name, float value)
    => server.SetShaderUniform(Handle, name, value);

  public void SetUniform(string name, Point2 value)
    => server.SetShaderUniform(Handle, name, value);

  public void SetUniform(string name, Point3 value)
    => server.SetShaderUniform(Handle, name, value);

  public void SetUniform(string name, Vector2 value)
    => server.SetShaderUniform(Handle, name, value);

  public void SetUniform(string name, Vector3 value)
    => server.SetShaderUniform(Handle, name, value);

  public void SetUniform(string name, Vector4 value)
    => server.SetShaderUniform(Handle, name, value);

  public void SetUniform(string name, Quaternion value)
    => server.SetShaderUniform(Handle, name, value);

  public void SetUniform(string name, in Matrix3x2 value)
    => server.SetShaderUniform(Handle, name, in value);

  public void SetUniform(string name, in Matrix4x4 value)
    => server.SetShaderUniform(Handle, name, in value);

  public void SetUniform(string name, Texture texture, int samplerSlot)
    => server.SetShaderTexture(Handle, name, texture.Handle, samplerSlot);

  /// <summary>Deletes and replaces the old shader with a new one.</summary>
  public void ReplaceShader(GraphicsHandle newHandle)
  {
    server.DeleteShader(Handle);
    Handle = newHandle;
  }

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      server.DeleteShader(Handle);
    }

    base.Dispose(managed);
  }
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="ShaderProgram"/>s.</summary>
public sealed class ShaderProgramLoader : AssetLoader<ShaderProgram>
{
  private readonly IGraphicsServer server;

  public ShaderProgramLoader(IGraphicsServer server)
  {
    this.server = server;
  }

  public override bool CanHandle(AssetLoaderContext context)
  {
    return base.CanHandle(context) && context.Path.Extension == ".shader";
  }

  public override async Task<ShaderProgram> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken)
  {
    var program = new ShaderProgram(server);
    var declaration = await context.LoadAsync<ShaderDeclaration>(context.Path, cancellationToken);

    server.CompileShader(program.Handle, declaration);

    if (context.IsHotReloadEnabled)
    {
      context.SubscribeToChanges<ShaderProgram>(ReloadAsync);
    }

    return program;
  }

  private async Task<ShaderProgram> ReloadAsync(AssetLoaderContext context, ShaderProgram program, CancellationToken cancellationToken = default)
  {
    var handle = server.CreateShader();
    var declaration = await context.LoadAsync<ShaderDeclaration>(context.Path, cancellationToken);

    server.CompileShader(handle, declaration);
    program.ReplaceShader(handle);

    return program;
  }
}
