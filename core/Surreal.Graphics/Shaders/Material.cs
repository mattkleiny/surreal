using Surreal.Assets;
using Surreal.Mathematics;

namespace Surreal.Graphics.Shaders;

/// <summary>Describes a single property of a <see cref="Material"/>.</summary>
public readonly record struct MaterialProperty<T>(string Name);

/// <summary>A material manages and batches GPU shader effects.</summary>
public sealed class Material
{
  private readonly ShaderProgram shader;

  public Material(ShaderProgram shader)
  {
    this.shader = shader;
  }

  public void SetProperty(MaterialProperty<int> property, int value)
    => shader.SetUniform(property.Name, value);

  public void SetProperty(MaterialProperty<float> property, float value)
    => shader.SetUniform(property.Name, value);

  public void SetProperty(MaterialProperty<Point2> property, Point2 value)
    => shader.SetUniform(property.Name, value);

  public void SetProperty(MaterialProperty<Point3> property, Point3 value)
    => shader.SetUniform(property.Name, value);

  public void SetProperty(MaterialProperty<Vector2> property, Vector2 value)
    => shader.SetUniform(property.Name, value);

  public void SetProperty(MaterialProperty<Vector3> property, Vector3 value)
    => shader.SetUniform(property.Name, value);

  public void SetProperty(MaterialProperty<Vector4> property, Vector4 value)
    => shader.SetUniform(property.Name, value);

  public void SetProperty(MaterialProperty<Quaternion> property, Quaternion value)
    => shader.SetUniform(property.Name, value);

  public void SetProperty(MaterialProperty<Matrix3x2> property, in Matrix3x2 value)
    => shader.SetUniform(property.Name, value);

  public void SetProperty(MaterialProperty<Matrix4x4> property, in Matrix4x4 value)
    => shader.SetUniform(property.Name, value);
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="ShaderProgram"/>s.</summary>
public sealed class MaterialLoader : AssetLoader<Material>
{
  public override async ValueTask<Material> LoadAsync(AssetLoaderContext context, ProgressToken progressToken = default)
  {
    var shaderProgram = await context.Manager.LoadAssetAsync<ShaderProgram>(context.Path, progressToken);

    return new Material(shaderProgram);
  }
}
