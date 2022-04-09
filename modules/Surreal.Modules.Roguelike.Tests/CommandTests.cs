using Surreal.Collections;

namespace Surreal;

public class CommandTests
{
  [Test]
  public void it_should_work()
  {
    var command = new MoveCommand(new Vector2(0f, -1f));
    var context = new CommandContext(new object(), new PropertyBag());

    var result = command.Execute(context);

    result.IsDone.Should().BeFalse();
    result.Command.Should().NotBeNull();
  }

  private sealed record MoveCommand(Vector2 Direction) : Command
  {
    public override CommandResult Execute(CommandContext context)
    {
      return CommandResult.Alternative(new MoveCommand(-Direction));
    }
  }
}
