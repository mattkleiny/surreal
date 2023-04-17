using Surreal.Graphics.Shaders;

namespace Surreal.Graphics.Materials;

/// <summary>
/// An effect is a specially configured <see cref="Material" />.
/// </summary>
public abstract class Effect : IDisposable
{
  protected Effect(Material material)
  {
    Material = material;
  }

  /// <summary>
  /// The underlying <see cref="Material" /> instance.
  /// </summary>
  public Material Material { get; }

  public IGraphicsServer Server => Material.Server;
  public ShaderProgram Shader => Material.Shader;
  public MaterialPropertySet Locals => Material.Locals;

  public BlendState Blending
  {
    get => Material.Blending;
    set => Material.Blending = value;
  }

  public virtual void Dispose()
  {
    Material.Dispose();
  }

  public static implicit operator Material(Effect decorator)
  {
    return decorator.Material;
  }
}
