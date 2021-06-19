using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Graphics.Textures;
using Surreal.IO;
using Surreal.Mathematics.Linear;

namespace Surreal.Graphics.Materials
{
  public sealed class Material : GraphicsResource
  {
    private readonly MaterialPass[] passes;

    public Material(params MaterialPass[] passes)
    {
      this.passes = passes;
    }

    public MaterialPass this[int index]
    {
      get
      {
        Debug.Assert(index >= 0 && index < passes.Length, "index >= 0 && index < passes.Length");

        return passes[index];
      }
    }
  }

  public sealed record MaterialPass(ShaderProgram Program)
  {
    public void SetProperty(MaterialProperty<int> property, int value)                => Program.SetUniform(property.Name, value);
    public void SetProperty(MaterialProperty<float> property, float value)            => Program.SetUniform(property.Name, value);
    public void SetProperty(MaterialProperty<Vector2> property, Vector2 value)        => Program.SetUniform(property.Name, value);
    public void SetProperty(MaterialProperty<Vector3> property, Vector3 value)        => Program.SetUniform(property.Name, value);
    public void SetProperty(MaterialProperty<Vector4> property, Vector4 value)        => Program.SetUniform(property.Name, value);
    public void SetProperty(MaterialProperty<Point2> property, Point2 value)          => Program.SetUniform(property.Name, value);
    public void SetProperty(MaterialProperty<Point3> property, Point3 value)          => Program.SetUniform(property.Name, value);
    public void SetProperty(MaterialProperty<Quaternion> property, Quaternion value)  => Program.SetUniform(property.Name, value);
    public void SetProperty(MaterialProperty<Matrix3x2> property, in Matrix3x2 value) => Program.SetUniform(property.Name, in value);
    public void SetProperty(MaterialProperty<Matrix4x4> property, in Matrix4x4 value) => Program.SetUniform(property.Name, in value);

    public void SetProperty(MaterialProperty<Texture> property, Texture value)         => throw new NotImplementedException();
    public void SetProperty(MaterialProperty<FrameBuffer> property, FrameBuffer value) => throw new NotImplementedException();
  }

  public sealed class MaterialLoader : AssetLoader<Material>
  {
    public override async Task<Material> LoadAsync(Path path, IAssetResolver context)
    {
      var program = await context.LoadAsset<ShaderProgram>(path);
      var pass    = new MaterialPass(program.Data);

      return new Material(pass);
    }
  }
}