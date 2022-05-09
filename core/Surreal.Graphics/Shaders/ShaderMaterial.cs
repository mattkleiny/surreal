using Surreal.Graphics.Meshes;
using Surreal.Mathematics;

namespace Surreal.Graphics.Shaders;

// TODO: work on me

/// <summary>A material is a <see cref="ShaderProgram"/> with unique uniform values stored efficiently on the GPU.</summary>
public sealed class ShaderMaterial : GraphicsResource
{
  private readonly IGraphicsServer server;
  private readonly ShaderProgram program;
  private readonly GraphicsHandle buffer;

  public ShaderMaterial(IGraphicsServer server, ShaderProgram program)
  {
    this.program = program;
    this.server  = server;

    buffer = server.CreateBuffer(BufferType.Uniform);
  }

  public ShaderProgram Program => program;

  public void SetUniform(string name, int value)
    => server.WriteBufferData<int>(buffer, BufferType.Uniform, stackalloc int[] { value }, BufferUsage.Static);

  public void SetUniform(string name, float value)
    => server.WriteBufferData<float>(buffer, BufferType.Uniform, stackalloc float[] { value }, BufferUsage.Static);

  public void SetUniform(string name, Point2 value)
    => server.WriteBufferData<int>(buffer, BufferType.Uniform, stackalloc int[] { value.X, value.Y }, BufferUsage.Static);

  public void SetUniform(string name, Point3 value)
    => server.WriteBufferData<int>(buffer, BufferType.Uniform, stackalloc int[] { value.X, value.Y, value.Z }, BufferUsage.Static);

  public void SetUniform(string name, Vector2 value)
    => server.WriteBufferData<float>(buffer, BufferType.Uniform, stackalloc float[] { value.X, value.Y }, BufferUsage.Static);

  public void SetUniform(string name, Vector3 value)
    => server.WriteBufferData<float>(buffer, BufferType.Uniform, stackalloc float[] { value.X, value.Y, value.Z }, BufferUsage.Static);

  public void SetUniform(string name, Vector4 value)
    => server.WriteBufferData<float>(buffer, BufferType.Uniform, stackalloc float[] { value.X, value.Y, value.Z, value.W }, BufferUsage.Static);

  public void SetUniform(string name, Quaternion value)
    => server.WriteBufferData<float>(buffer, BufferType.Uniform, stackalloc float[] { value.X, value.Y, value.Z, value.W }, BufferUsage.Static);

  protected override void Dispose(bool managed)
  {
    if (managed)
    {
      server.DeleteBuffer(buffer);
    }

    base.Dispose(managed);
  }
}
