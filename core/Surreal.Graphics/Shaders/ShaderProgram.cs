using Surreal.Assets;
using Surreal.Graphics.Meshes;
using Surreal.Mathematics;

namespace Surreal.Graphics.Shaders;

/// <summary>A low-level shader program on the GPU.</summary>
public sealed class ShaderProgram : GraphicsResource
{
  private readonly IGraphicsServer server;
  private readonly GraphicsHandle handle;

  public ShaderProgram(IGraphicsServer server, ShaderDeclaration declaration)
  {
    this.server = server;
    handle = server.CreateShader();

    server.CompileShader(handle, declaration);
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

  public void SetUniform(string name, int value) => server.SetShaderUniform(handle, name, value);
  public void SetUniform(string name, float value) => server.SetShaderUniform(handle, name, value);
  public void SetUniform(string name, Point2 value) => server.SetShaderUniform(handle, name, value);
  public void SetUniform(string name, Point3 value) => server.SetShaderUniform(handle, name, value);
  public void SetUniform(string name, Vector2 value) => server.SetShaderUniform(handle, name, value);
  public void SetUniform(string name, Vector3 value) => server.SetShaderUniform(handle, name, value);
  public void SetUniform(string name, Vector4 value) => server.SetShaderUniform(handle, name, value);
  public void SetUniform(string name, Quaternion value) => server.SetShaderUniform(handle, name, value);
  public void SetUniform(string name, in Matrix3x2 value) => server.SetShaderUniform(handle, name, in value);
  public void SetUniform(string name, in Matrix4x4 value) => server.SetShaderUniform(handle, name, in value);

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      server.DeleteShader(handle);
    }

    base.Dispose(managed);
  }
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="ShaderProgram"/>s.</summary>
public sealed class ShaderProgramLoader : AssetLoader<ShaderProgram>
{
  private readonly IGraphicsServer server;
  private readonly ImmutableHashSet<string> extensions;

  public ShaderProgramLoader(IGraphicsServer server, params string[] extensions)
    : this(server, extensions.AsEnumerable())
  {
  }

  public ShaderProgramLoader(IGraphicsServer server, IEnumerable<string> extensions)
  {
    this.server = server;
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

    return new ShaderProgram(server, declaration);
  }
}
