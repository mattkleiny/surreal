using NUnit.Framework;
using Surreal.Testing;

namespace Surreal
{
  public sealed class PrototypeGameTests : GameTestCase<PrototypeGameTests.TestGame>
  {
    [Test]
    public void it_should_create_a_valid_audio_device()
    {
      Assert.IsNotNull(Game.AudioDevice);
    }

    [Test]
    public void it_should_create_a_valid_compute_device()
    {
      Assert.IsNotNull(Game.ComputeDevice);
    }

    [Test]
    public void it_should_create_a_valid_graphics_device()
    {
      Assert.IsNotNull(Game.GraphicsDevice);
    }

    [Test]
    public void it_should_create_a_valid_input_manager()
    {
      Assert.IsNotNull(Game.InputManager);
    }

    [Test]
    public void it_should_create_a_valid_keyboard()
    {
      Assert.IsNotNull(Game.Keyboard);
    }

    [Test]
    public void it_should_create_a_valid_mouse()
    {
      Assert.IsNotNull(Game.Mouse);
    }

    [Test]
    public void it_should_create_a_valid_screen_manager()
    {
      Assert.IsNotNull(Game.Screens);
    }

    [Test]
    public void it_should_create_a_valid_console()
    {
      Assert.IsNotNull(Game.Console);
    }

    public sealed class TestGame : PrototypeGame
    {
    }
  }
}
