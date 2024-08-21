using Surreal.Input.Keyboard;

namespace Surreal.Input;

public sealed record InputAction(string Name)
{
  public static InputAction Jump { get; } = new("jump");
  public static InputAction Exit { get; } = new("exit");
}

public class InputManagerTests
{
  [Test]
  public void it_should_map_events_to_actions()
  {
    var events = new InputEventSubject();
    var manager = new InputManager<InputAction>(events);

    var wasJumpTriggered = false;

    manager.ActionTriggered += action =>
    {
      Console.WriteLine($"Action triggered: {action.Name}");

      if (action == InputAction.Jump)
      {
        wasJumpTriggered = true;
      }
    };

    manager.BindAction(InputAction.Jump, new KeyPressEvent(Key.Space, true));
    manager.BindAction(InputAction.Exit, new KeyPressEvent(Key.Escape, true));

    events.NotifyNext(new KeyPressEvent(Key.Space, IsPressed: true));

    manager.Flush();

    Assert.IsTrue(wasJumpTriggered);
  }
}
