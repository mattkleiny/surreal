using Surreal.IO;

namespace Surreal.Graphics.Materials;

public class ShaderProgramTests
{
  [Test]
  public void it_should_not_set_uniform_if_location_is_invalid()
  {
    var backend = Substitute.For<IGraphicsBackend>();
    var program = new ShaderProgram(backend);

    backend.GetShaderUniformLocation(Arg.Any<GraphicsHandle>(), Arg.Any<string>()).Returns(-1);

    program.SetUniform("u_rotation", Quaternion.Identity);

    backend.Received(0).SetShaderUniform(Arg.Any<GraphicsHandle>(), Arg.Any<int>(), Arg.Any<Quaternion>());
  }

  [Test]
  public void it_should_set_uniform_if_location_is_valid()
  {
    var backend = Substitute.For<IGraphicsBackend>();
    var program = new ShaderProgram(backend);

    backend.GetShaderUniformLocation(Arg.Any<GraphicsHandle>(), Arg.Any<string>()).Returns(5);

    program.SetUniform("u_rotation", Quaternion.Identity);

    backend.Received(1).SetShaderUniform(Arg.Any<GraphicsHandle>(), Arg.Any<int>(), Arg.Any<Quaternion>());
  }

  [Test]
  [TestCase("local://Assets/External/shaders/test01.glsl")]
  public void it_should_parse_shader_programs(VirtualPath path)
  {
    var backend = IGraphicsBackend.Null;
    var program = ShaderProgram.Load(backend, path);

    program.Should().NotBeNull();
  }
}
