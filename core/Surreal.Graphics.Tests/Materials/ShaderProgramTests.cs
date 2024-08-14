using Surreal.IO;

namespace Surreal.Graphics.Materials;

public class ShaderProgramTests
{
  [Test]
  public void it_should_not_set_uniform_if_location_is_invalid()
  {
    var device = Substitute.For<IGraphicsDevice>();
    var program = new ShaderProgram(device);

    device.GetShaderUniformLocation(Arg.Any<GraphicsHandle>(), Arg.Any<string>()).Returns(-1);

    program.SetUniform("u_rotation", Quaternion.Identity);
  }

  [Test]
  public void it_should_set_uniform_if_location_is_valid()
  {
    var device = Substitute.For<IGraphicsDevice>();
    var program = new ShaderProgram(device);

    device.GetShaderUniformLocation(Arg.Any<GraphicsHandle>(), Arg.Any<string>()).Returns(5);

    program.SetUniform("u_rotation", Quaternion.Identity);
  }

  [Test]
  [TestCase("local://Assets/External/shaders/test01.glsl")]
  public void it_should_parse_shader_programs(VirtualPath path)
  {
    var device = IGraphicsDevice.Null;
    var program = ShaderProgram.Load(device, path);

    program.Should().NotBeNull();
  }
}
