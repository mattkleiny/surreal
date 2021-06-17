using System;
using System.Numerics;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Data;
using Surreal.Graphics.Meshes;
using Surreal.Mathematics.Linear;

namespace Surreal.Graphics.Materials {
  public enum ShaderProgramType {
    Sprite,
    Screen,
  }

  public abstract class ShaderProgram : GraphicsResource {
    public abstract void Bind(VertexAttributeSet attributes);

    public abstract void SetUniform(string alias, int scalar);
    public abstract void SetUniform(string alias, float scalar);
    public abstract void SetUniform(string alias, Point2 point);
    public abstract void SetUniform(string alias, Point3 point);
    public abstract void SetUniform(string alias, Vector2 vector);
    public abstract void SetUniform(string alias, Vector3 vector);
    public abstract void SetUniform(string alias, Vector4 vector);
    public abstract void SetUniform(string alias, Quaternion quaternion);
    public abstract void SetUniform(string alias, in Matrix2x2 matrix);
    public abstract void SetUniform(string alias, in Matrix3x2 matrix);
    public abstract void SetUniform(string alias, in Matrix4x4 matrix);

    public sealed class Loader : AssetLoader<ShaderProgram> {
      private readonly IGraphicsDevice device;
      private readonly bool            hotReloading;

      public Loader(IGraphicsDevice device, bool hotReloading) {
        this.device       = device;
        this.hotReloading = hotReloading;
      }

      public override Task<ShaderProgram> LoadAsync(Path path, IAssetLoaderContext context) {
        throw new NotImplementedException();
      }
    }
  }
}