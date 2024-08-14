using Surreal.Colors;

namespace Surreal.Graphics.Materials;

public class MaterialTests
{
  private static ShaderProperty<Matrix4x4> Transform { get; } = new("u_transform");
  private static ShaderProperty<Color32> ClearColor { get; } = new("u_clearColor");

  [Test]
  public void it_should_apply_uniforms_to_backend_when_rendering()
  {
    var device = Substitute.For<IGraphicsDevice>();

    using var shader = new ShaderProgram(device);
    using var material = new Material(device, shader);

    material.BlendState = BlendState.OneMinusSourceAlpha;

    material.ApplyMaterial();

    device.Received().SetActiveShader(Arg.Any<GraphicsHandle>());
    device.Received().SetBlendState(material.BlendState);
  }

  [Test]
  public void it_should_dispose_shader_if_it_owns_it()
  {
    var device = Substitute.For<IGraphicsDevice>();

    using var shader = new ShaderProgram(device);
    var material = new Material(device, shader, ownsShader: true)
    {
      BlendState = BlendState.OneMinusSourceAlpha
    };

    material.Dispose();

    device.Received(1).DeleteShader(Arg.Any<GraphicsHandle>());
  }

  [Test]
  public void it_should_not_dispose_shader_if_it_doesnt_own_it()
  {
    var device = Substitute.For<IGraphicsDevice>();

    using var shader = new ShaderProgram(device);
    var material = new Material(device, shader, ownsShader: false)
    {
      BlendState = BlendState.OneMinusSourceAlpha
    };

    material.Dispose();

    device.Received(0).DeleteShader(Arg.Any<GraphicsHandle>());
  }

  [Test]
  public void it_should_transfer_active_shader_on_apply()
  {
    var device = Substitute.For<IGraphicsDevice>();

    using var shader = new ShaderProgram(device);
    using var material = new Material(device, shader);

    material.ApplyMaterial();

    device.Received(1).SetActiveShader(shader.Handle);
  }

  [Test]
  public void it_should_transfer_uniforms_on_apply()
  {
    var device = Substitute.For<IGraphicsDevice>();

    using var shader = new ShaderProgram(device);
    using var material = new Material(device, shader);

    device.GetShaderUniformLocation(shader.Handle, Arg.Any<string>()).Returns(2);

    material.Uniforms.Set(Transform, Matrix4x4.Identity);
    material.Uniforms.Set(ClearColor, Color32.White);

    material.ApplyMaterial();
  }

  [Test]
  public void it_should_transfer_active_blend_state_on_apply()
  {
    var device = Substitute.For<IGraphicsDevice>();

    using var shader = new ShaderProgram(device);
    using var material = new Material(device, shader);

    material.BlendState = BlendState.OneMinusSourceAlpha;

    material.ApplyMaterial();

    device.Received(1).SetBlendState(material.BlendState);
  }
}
