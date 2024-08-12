using Surreal.Colors;

namespace Surreal.Graphics.Materials;

public class MaterialTests
{
  private static UniformProperty<Matrix4x4> Transform { get; } = new("u_transform");
  private static UniformProperty<Color32> ClearColor { get; } = new("u_clearColor");

  [Test]
  public void it_should_apply_uniforms_to_backend_when_rendering()
  {
    var backend = Substitute.For<IGraphicsBackend>();

    using var shader = new ShaderProgram(backend);
    using var material = new Material(backend, shader);

    material.BlendState = BlendState.OneMinusSourceAlpha;

    material.ApplyMaterial();

    backend.Received().SetActiveShader(Arg.Any<GraphicsHandle>());
    backend.Received().SetBlendState(material.BlendState);
  }

  [Test]
  public void it_should_dispose_shader_if_it_owns_it()
  {
    var backend = Substitute.For<IGraphicsBackend>();

    using var shader = new ShaderProgram(backend);
    var material = new Material(backend, shader, ownsShader: true)
    {
      BlendState = BlendState.OneMinusSourceAlpha
    };

    material.Dispose();

    backend.Received(1).DeleteShader(Arg.Any<GraphicsHandle>());
  }

  [Test]
  public void it_should_not_dispose_shader_if_it_doesnt_own_it()
  {
    var backend = Substitute.For<IGraphicsBackend>();

    using var shader = new ShaderProgram(backend);
    var material = new Material(backend, shader, ownsShader: false)
    {
      BlendState = BlendState.OneMinusSourceAlpha
    };

    material.Dispose();

    backend.Received(0).DeleteShader(Arg.Any<GraphicsHandle>());
  }

  [Test]
  public void it_should_transfer_active_shader_on_apply()
  {
    var backend = Substitute.For<IGraphicsBackend>();

    using var shader = new ShaderProgram(backend);
    using var material = new Material(backend, shader);

    material.ApplyMaterial();

    backend.Received(1).SetActiveShader(shader.Handle);
  }

  [Test]
  public void it_should_transfer_uniforms_on_apply()
  {
    var backend = Substitute.For<IGraphicsBackend>();

    using var shader = new ShaderProgram(backend);
    using var material = new Material(backend, shader);

    backend.GetShaderUniformLocation(shader.Handle, Arg.Any<string>()).Returns(2);

    material.Uniforms.Set(Transform, Matrix4x4.Identity);
    material.Uniforms.Set(ClearColor, Color32.White);

    material.ApplyMaterial();

    backend.Received(1).SetShaderUniform(shader.Handle, Arg.Any<int>(), Matrix4x4.Identity);
    backend.Received(1).SetShaderUniform(shader.Handle, Arg.Any<int>(), Color32.White);
  }

  [Test]
  public void it_should_transfer_active_blend_state_on_apply()
  {
    var backend = Substitute.For<IGraphicsBackend>();

    using var shader = new ShaderProgram(backend);
    using var material = new Material(backend, shader);

    material.BlendState = BlendState.OneMinusSourceAlpha;

    material.ApplyMaterial();

    backend.Received(1).SetBlendState(material.BlendState);
  }
}
