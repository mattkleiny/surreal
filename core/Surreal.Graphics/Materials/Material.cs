using Surreal.Assets;
using Surreal.Graphics.Textures;
using Surreal.IO;
using Surreal.Mathematics;

namespace Surreal.Graphics.Materials;

/// <summary>A material manages and batches GPU shader effects.</summary>
public sealed class Material : GraphicsResource
{
  public Material(ShaderProgram program)
  {
    Program = program;
  }

  public ShaderProgram Program { get; }

  public void SetProperty(MaterialProperty<int> property, int value)     => Program.SetUniform(property.Name, value);
  public void SetProperty(MaterialProperty<float> property, float value) => Program.SetUniform(property.Name, value);

  public void SetProperty(MaterialProperty<Vector2> property, Vector2 value) =>
    Program.SetUniform(property.Name, value);

  public void SetProperty(MaterialProperty<Vector3> property, Vector3 value) =>
    Program.SetUniform(property.Name, value);

  public void SetProperty(MaterialProperty<Vector4> property, Vector4 value) =>
    Program.SetUniform(property.Name, value);

  public void SetProperty(MaterialProperty<Vector2I> property, Vector2I value) =>
    Program.SetUniform(property.Name, value);

  public void SetProperty(MaterialProperty<Vector3I> property, Vector3I value) =>
    Program.SetUniform(property.Name, value);

  public void SetProperty(MaterialProperty<Quaternion> property, Quaternion value) =>
    Program.SetUniform(property.Name, value);

  public void SetProperty(MaterialProperty<Matrix3x2> property, in Matrix3x2 value) =>
    Program.SetUniform(property.Name, in value);

  public void SetProperty(MaterialProperty<Matrix4x4> property, in Matrix4x4 value) =>
    Program.SetUniform(property.Name, in value);

  public void SetProperty(MaterialProperty<Texture> property, Texture value) => throw new NotImplementedException();

  public void SetProperty(MaterialProperty<RenderTexture> property, RenderTexture value) =>
    throw new NotImplementedException();
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="Material"/>s.</summary>
public sealed class MaterialLoader : AssetLoader<Material>
{
  public override async Task<Material> LoadAsync(VirtualPath path, IAssetContext context)
  {
    var program = await context.LoadAsset<ShaderProgram>(path);

    return new Material(program);
  }
}
