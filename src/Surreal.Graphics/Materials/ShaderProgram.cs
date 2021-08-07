using System;
using System.Numerics;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Graphics.Meshes;
using Surreal.IO;
using Surreal.Mathematics.Linear;

namespace Surreal.Graphics.Materials
{
  public abstract class ShaderProgram : GraphicsResource
  {
    public abstract void Bind(VertexDescriptorSet descriptors);

    public abstract void SetUniform(string name, int scalar);
    public abstract void SetUniform(string name, float scalar);
    public abstract void SetUniform(string name, Point2 point);
    public abstract void SetUniform(string name, Point3 point);
    public abstract void SetUniform(string name, Vector2 vector);
    public abstract void SetUniform(string name, Vector3 vector);
    public abstract void SetUniform(string name, Vector4 vector);
    public abstract void SetUniform(string name, Quaternion quaternion);
    public abstract void SetUniform(string name, in Matrix3x2 matrix);
    public abstract void SetUniform(string name, in Matrix4x4 matrix);
  }

  public sealed class ShaderProgramLoader : AssetLoader<ShaderProgram>
  {
    private readonly IGraphicsDevice device;
    private readonly bool            hotReloading;

    public ShaderProgramLoader(IGraphicsDevice device, bool hotReloading)
    {
      this.device       = device;
      this.hotReloading = hotReloading;
    }

    public override Task<ShaderProgram> LoadAsync(Path path, IAssetResolver context)
    {
      throw new NotImplementedException();
    }
  }
}