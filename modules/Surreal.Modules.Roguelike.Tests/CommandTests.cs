using Surreal.Collections;

namespace Surreal;

public class CommandTests
{
  [Test]
  public async Task it_should_work()
  {
    var command = new MoveCommand(new Vector2(0f, -1f));
    var context = new CommandContext(new object(), new PropertyBag());

    var result = await command.ExecuteAsync(context);

    result.Command.Should().NotBeNull();
  }

  private sealed record MoveCommand(Vector2 Direction) : Command
  {
    public override ValueTask<CommandResult> ExecuteAsync(CommandContext context)
    {
      return new(CommandResult.Alternative(new MoveCommand(-Direction)));
    }
  }
}
