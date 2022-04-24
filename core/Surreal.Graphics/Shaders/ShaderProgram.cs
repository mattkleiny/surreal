using Surreal.Assets;
using Surreal.Mathematics;

namespace Surreal.Graphics.Shaders;

/// <summary>Describes a single uniform of a <see cref="ShaderProgram"/>.</summary>
public readonly record struct ShaderUniform<T>(string Name);

/// <summary>A low-level shader program on the GPU.</summary>
public sealed class ShaderProgram : GraphicsResource
{
  private readonly IGraphicsServer server;

  public ShaderProgram(IGraphicsServer server)
  {
    this.server = server;

    Handle = server.CreateShader();
  }

  public ShaderProgram(IGraphicsServer server, ShaderDeclaration declaration)
    : this(server)
  {
    server.CompileShader(Handle, declaration);
  }

  public GraphicsHandle Handle { get; }

  public void SetUniform(ShaderUniform<int> uniform, int value)
    => server.SetShaderUniform(Handle, uniform.Name, value);

  public void SetUniform(ShaderUniform<float> uniform, float value)
    => server.SetShaderUniform(Handle, uniform.Name, value);

  public void SetUniform(ShaderUniform<Point2> uniform, Point2 value)
    => server.SetShaderUniform(Handle, uniform.Name, value);

  public void SetUniform(ShaderUniform<Point3> uniform, Point3 value)
    => server.SetShaderUniform(Handle, uniform.Name, value);

  public void SetUniform(ShaderUniform<Vector2> uniform, Vector2 value)
    => server.SetShaderUniform(Handle, uniform.Name, value);

  public void SetUniform(ShaderUniform<Vector3> uniform, Vector3 value)
    => server.SetShaderUniform(Handle, uniform.Name, value);

  public void SetUniform(ShaderUniform<Vector4> uniform, Vector4 value)
    => server.SetShaderUniform(Handle, uniform.Name, value);

  public void SetUniform(ShaderUniform<Quaternion> uniform, Quaternion value)
    => server.SetShaderUniform(Handle, uniform.Name, value);

  public void SetUniform(ShaderUniform<Matrix3x2> uniform, in Matrix3x2 value)
    => server.SetShaderUniform(Handle, uniform.Name, in value);

  public void SetUniform(ShaderUniform<Matrix4x4> uniform, in Matrix4x4 value)
    => server.SetShaderUniform(Handle, uniform.Name, in value);

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
  private readonly ImmutableHashSet<string> extensions;

  public ShaderProgramLoader(IGraphicsServer server, params string[] extensions)
    : this(server, extensions.AsEnumerable())
  {
  }

  public ShaderProgramLoader(IGraphicsServer server, IEnumerable<string> extensions)
  {
    this.server     = server;
    this.extensions = extensions.ToImmutableHashSet();
  }

  public override bool CanHandle(AssetLoaderContext context)
  {
    return base.CanHandle(context) && extensions.Contains(context.Path.Extension);
  }

  public override async ValueTask<ShaderProgram> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken = default)
  {
    var program = new ShaderProgram(server);
    var declaration = await context.Manager.LoadAssetAsync<ShaderDeclaration>(context.Path, cancellationToken);

    server.CompileShader(program.Handle, declaration);

    return program;
  }
}
