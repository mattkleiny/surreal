using System.Diagnostics.CodeAnalysis;
using Surreal.Graphics.Shaders;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;

namespace Surreal.Graphics.Rendering;

/// <summary>A material is a set of shader properties and textures that can be used for rendering objects.</summary>
public sealed class Material
{
  private readonly Dictionary<string, Property> properties = new();

  public Material(ShaderProgram shaderProgram)
  {
    ShaderProgram = shaderProgram;
  }

  public ShaderProgram ShaderProgram { get; }

  public void SetProperty(string name, int value)
    => properties[name] = new IntProperty(value);

  public void SetProperty(string name, float value)
    => properties[name] = new FloatProperty(value);

  public void SetProperty(string name, Point2 value)
    => properties[name] = new Point2Property(value);

  public void SetProperty(string name, Point3 value)
    => properties[name] = new Point3Property(value);

  public void SetProperty(string name, Vector2 value)
    => properties[name] = new Vector2Property(value);

  public void SetProperty(string name, Vector3 value)
    => properties[name] = new Vector3Property(value);

  public void SetProperty(string name, Vector4 value)
    => properties[name] = new Vector4Property(value);

  public void SetProperty(string name, Matrix3x2 value)
    => properties[name] = new Matrix3x2Property(value);

  public void SetProperty(string name, Matrix4x4 value)
    => properties[name] = new Matrix4x4Property(value);

  public void SetProperty(string name, Texture value, int sampler)
    => properties[name] = new TextureProperty(value, sampler);

  /// <summary>Clears all properties from the material.</summary>
  public void ClearProperties()
    => properties.Clear();

  /// <summary>Applies the material properties to the <see cref="ShaderProgram"/>.</summary>
  public void Apply()
  {
    foreach (var (name, property) in properties)
    {
      property.SetUniform(name, ShaderProgram);
    }
  }

  /// <summary>Represents a property in a <see cref="Material"/>.</summary>
  private abstract record Property()
  {
    public abstract void SetUniform(string name, ShaderProgram program);
  }

  private sealed record IntProperty(int Value) : Property
  {
    public override void SetUniform(string name, ShaderProgram program)
    {
      program.SetUniform(name, Value);
    }
  }

  private sealed record FloatProperty(float Value) : Property
  {
    public override void SetUniform(string name, ShaderProgram program)
    {
      program.SetUniform(name, Value);
    }
  }

  private sealed record Point2Property(Point2 Value) : Property
  {
    public override void SetUniform(string name, ShaderProgram program)
    {
      program.SetUniform(name, Value);
    }
  }

  private sealed record Point3Property(Point3 Value) : Property
  {
    public override void SetUniform(string name, ShaderProgram program)
    {
      program.SetUniform(name, Value);
    }
  }

  private sealed record Vector2Property(Vector2 Value) : Property
  {
    public override void SetUniform(string name, ShaderProgram program)
    {
      program.SetUniform(name, Value);
    }
  }

  private sealed record Vector3Property(Vector3 Value) : Property
  {
    public override void SetUniform(string name, ShaderProgram program)
    {
      program.SetUniform(name, Value);
    }
  }

  private sealed record Vector4Property(Vector4 Value) : Property
  {
    public override void SetUniform(string name, ShaderProgram program)
    {
      program.SetUniform(name, Value);
    }
  }

  [SuppressMessage("ReSharper", "InconsistentNaming")]
  private sealed record Matrix3x2Property(Matrix3x2 Value) : Property
  {
    public override void SetUniform(string name, ShaderProgram program)
    {
      program.SetUniform(name, Value);
    }
  }

  [SuppressMessage("ReSharper", "InconsistentNaming")]
  private sealed record Matrix4x4Property(Matrix4x4 Value) : Property
  {
    public override void SetUniform(string name, ShaderProgram program)
    {
      program.SetUniform(name, Value);
    }
  }

  private sealed record TextureProperty(Texture Texture, int Sampler) : Property
  {
    public override void SetUniform(string name, ShaderProgram program)
    {
      program.SetUniform(name, Texture, Sampler);
    }
  }
}
