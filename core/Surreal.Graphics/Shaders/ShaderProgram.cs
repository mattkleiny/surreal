using Surreal.Assets;
using Surreal.Graphics.Meshes;
using Surreal.Mathematics;

namespace Surreal.Graphics.Shaders;

/// <summary>A low-level shader program on the GPU.</summary>
public sealed class ShaderProgram : GraphicsResource
{
  private readonly IGraphicsServer server;

  private readonly GraphicsId id;

  public ShaderProgram(IGraphicsServer server)
  {
    this.server = server;

    id = server.Shaders.CreateShader();
  }

  public void Bind(VertexDescriptorSet descriptors)
  {
    // GL.UseProgram(Id);
    //
    // for (var i = 0; i < descriptors.Length; i++)
    // {
    //   var attribute = descriptors[i];
    //   var location  = GL.GetAttribLocation(Id, attribute.Alias);
    //
    //   if (location == -1) continue; // attribute undefined in the shader? just move on
    //
    //   GL.VertexAttribPointer(
    //     index: (uint) location,
    //     size: attribute.Count,
    //     type: ConvertVertexType(attribute.Type),
    //     normalized: attribute.Normalized,
    //     stride: descriptors.Stride,
    //     offset: attribute.Offset
    //   );
    //   GL.EnableVertexAttribArray((uint) location);
    // }

    throw new NotImplementedException();
  }

  public void Compile(ShaderDeclaration declaration)
  {
    server.Shaders.CompileShader(id, declaration);
  }

  public void SetUniform(string name, int value)          => server.Shaders.SetShaderUniform(id, name, value);
  public void SetUniform(string name, float value)        => server.Shaders.SetShaderUniform(id, name, value);
  public void SetUniform(string name, Vector2I value)     => server.Shaders.SetShaderUniform(id, name, value);
  public void SetUniform(string name, Vector3I value)     => server.Shaders.SetShaderUniform(id, name, value);
  public void SetUniform(string name, Vector2 value)      => server.Shaders.SetShaderUniform(id, name, value);
  public void SetUniform(string name, Vector3 value)      => server.Shaders.SetShaderUniform(id, name, value);
  public void SetUniform(string name, Vector4 value)      => server.Shaders.SetShaderUniform(id, name, value);
  public void SetUniform(string name, Quaternion value)   => server.Shaders.SetShaderUniform(id, name, value);
  public void SetUniform(string name, in Matrix3x2 value) => server.Shaders.SetShaderUniform(id, name, in value);
  public void SetUniform(string name, in Matrix4x4 value) => server.Shaders.SetShaderUniform(id, name, in value);

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      server.Shaders.DeleteShader(id);
    }

    base.Dispose(managed);
  }
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="ShaderProgram"/>s.</summary>
public sealed class ShaderProgramLoader : AssetLoader<ShaderProgram>
{
  private readonly IGraphicsDevice          device;
  private readonly ImmutableHashSet<string> extensions;

  public ShaderProgramLoader(IGraphicsDevice device, params string[] extensions)
    : this(device, extensions.AsEnumerable())
  {
  }

  public ShaderProgramLoader(IGraphicsDevice device, IEnumerable<string> extensions)
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

    var declaration = await context.Manager.LoadAssetAsync<ShaderDeclaration>(context.Path, progressToken);
    var program     = new ShaderProgram(device.Server);

    try
    {
      program.Compile(declaration);
      return program;
    }
    catch (Exception)
    {
      program.Dispose();
      throw;
    }
  }
}
