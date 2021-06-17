using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Data.VFS;
using Surreal.Graphics.Textures;
using Surreal.Mathematics.Linear;

namespace Surreal.Graphics.Materials {
  public sealed class Material : GraphicsResource {
    private readonly Pass[] passes;

    public Material(params Pass[] passes) {
      this.passes = passes;
    }

    public Pass this[int index] {
      get {
        Debug.Assert(index >= 0 && index < passes.Length, "index >= 0 && index < passes.Length");

        return passes[index];
      }
    }

    public sealed class Pass {
      private readonly ShaderProgram program;

      public Pass(ShaderProgram program) {
        this.program = program;
      }

      public void SetProperty(MaterialProperty<int> property, int value)                => program.SetUniform(property.Name, value);
      public void SetProperty(MaterialProperty<float> property, float value)            => program.SetUniform(property.Name, value);
      public void SetProperty(MaterialProperty<Vector2> property, Vector2 value)        => program.SetUniform(property.Name, value);
      public void SetProperty(MaterialProperty<Vector3> property, Vector3 value)        => program.SetUniform(property.Name, value);
      public void SetProperty(MaterialProperty<Vector4> property, Vector4 value)        => program.SetUniform(property.Name, value);
      public void SetProperty(MaterialProperty<Point2> property, Point2 value)          => program.SetUniform(property.Name, value);
      public void SetProperty(MaterialProperty<Point3> property, Point3 value)          => program.SetUniform(property.Name, value);
      public void SetProperty(MaterialProperty<Quaternion> property, Quaternion value)  => program.SetUniform(property.Name, value);
      public void SetProperty(MaterialProperty<Matrix2x2> property, in Matrix2x2 value) => program.SetUniform(property.Name, in value);
      public void SetProperty(MaterialProperty<Matrix3x2> property, in Matrix3x2 value) => program.SetUniform(property.Name, in value);
      public void SetProperty(MaterialProperty<Matrix4x4> property, in Matrix4x4 value) => program.SetUniform(property.Name, in value);

      public void SetProperty(MaterialProperty<Texture> property, Texture value)         => throw new NotImplementedException();
      public void SetProperty(MaterialProperty<FrameBuffer> property, FrameBuffer value) => throw new NotImplementedException();
    }

    public sealed class Loader : AssetLoader<Material> {
      public override async Task<Material> LoadAsync(Path path, IAssetLoaderContext context) {
        var program = await context.GetAsync<ShaderProgram>(path);
        var pass    = new Pass(program);

        return new Material(pass);
      }
    }
  }
}