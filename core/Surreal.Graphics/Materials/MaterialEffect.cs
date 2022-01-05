using Surreal.Mathematics;

namespace Surreal.Graphics.Materials;

/// <summary>An effect is a set of <see cref="MaterialProperty{T}"/>s that can be applied on demand.</summary>
public sealed record MaterialEffect : IEnumerable
{
  private readonly List<Action<Material>> actions = new();

  public void Add(MaterialProperty<int> property, int value)               => actions.Add(_ => _.SetProperty(property, value));
  public void Add(MaterialProperty<float> property, float value)           => actions.Add(_ => _.SetProperty(property, value));
  public void Add(MaterialProperty<Vector2> property, Vector2 value)       => actions.Add(_ => _.SetProperty(property, value));
  public void Add(MaterialProperty<Vector3> property, Vector3 value)       => actions.Add(_ => _.SetProperty(property, value));
  public void Add(MaterialProperty<Vector4> property, Vector4 value)       => actions.Add(_ => _.SetProperty(property, value));
  public void Add(MaterialProperty<Vector2I> property, Vector2I value)     => actions.Add(_ => _.SetProperty(property, value));
  public void Add(MaterialProperty<Vector3I> property, Vector3I value)     => actions.Add(_ => _.SetProperty(property, value));
  public void Add(MaterialProperty<Quaternion> property, Quaternion value) => actions.Add(_ => _.SetProperty(property, value));
  public void Add(MaterialProperty<Matrix3x2> property, Matrix3x2 value)   => actions.Add(_ => _.SetProperty(property, value));
  public void Add(MaterialProperty<Matrix4x4> property, Matrix4x4 value)   => actions.Add(_ => _.SetProperty(property, value));

  public void Clear() => actions.Clear();

  public void ApplyToMaterial(Material material)
  {
    foreach (var action in actions)
    {
      action(material);
    }
  }

  // N.B: only to allow initialization syntax
  IEnumerator IEnumerable.GetEnumerator() => actions.GetEnumerator();
}
