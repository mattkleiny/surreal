﻿using Surreal.Colors;

namespace Surreal.Graphics.Materials;

public class ShaderPropertyTests
{
  private static ShaderProperty<Matrix4x4> Transform { get; } = new("u_transform");
  private static ShaderProperty<Color32> ClearColor { get; } = new("u_clearColor");
  private static ShaderProperty<Vector3> LightPosition { get; } = new("u_lightPosition");

  [Test]
  public void it_should_read_and_write_matrix()
  {
    var propertySet = new ShaderPropertySet();

    propertySet.Set(Transform, Matrix4x4.Identity);
    propertySet.GetOrThrow(Transform).Should().Be(Matrix4x4.Identity);
  }

  [Test]
  public void it_should_read_and_write_color()
  {
    var propertySet = new ShaderPropertySet();

    propertySet.Set(ClearColor, Color32.White);
    propertySet.GetOrThrow(ClearColor).Should().Be(Color32.White);
  }

  [Test]
  public void it_should_read_and_write_vector()
  {
    var propertySet = new ShaderPropertySet();

    propertySet.Set(LightPosition, Vector3.One);
    propertySet.GetOrThrow(LightPosition).Should().Be(Vector3.One);
  }
}
