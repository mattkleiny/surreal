namespace Surreal.Graphics.Materials;

public class MaterialTests
{
  [Test]
  public void it_should_apply_uniforms_to_backend_when_rendering()
  {
    var backend = Substitute.For<IGraphicsBackend>();

    using var shader = new ShaderProgram(backend);
    using var material = new Material(shader);

    material.BlendState = BlendState.OneMinusSourceAlpha;

    material.Apply(backend);

    backend.Received().SetActiveShader(Arg.Any<GraphicsHandle>());
    backend.Received().SetBlendState(material.BlendState);
  }

  [Test]
  public void it_should_dispose_shader_if_it_owns_it()
  {
    var backend = Substitute.For<IGraphicsBackend>();

    using var shader = new ShaderProgram(backend);
    var material = new Material(shader, ownsShader: true)
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
    var material = new Material(shader, ownsShader: false)
    {
      BlendState = BlendState.OneMinusSourceAlpha
    };

    material.Dispose();

    backend.Received(0).DeleteShader(Arg.Any<GraphicsHandle>());
  }
}
