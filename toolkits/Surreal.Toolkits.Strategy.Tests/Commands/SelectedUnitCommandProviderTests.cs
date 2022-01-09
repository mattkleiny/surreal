using Surreal.Selection;

namespace Surreal.Commands;

public class SelectedUnitCommandProviderTests
{
  [Test]
  public void it_should_yield_commands_from_selected_units()
  {
    var selectionManager = new UnitSelectionManager();
    var commandProvider  = new SelectedUnitCommandProvider(selectionManager);

    selectionManager.AddToSelection(new TestUnit());
    selectionManager.AddToSelection(new TestUnit());

    var commands = commandProvider.GetCommands(new CommandContext());

    Assert.That(commands, Is.Not.Empty);
  }

  private sealed class TestUnit : ISelectableUnit, ICommandProvider
  {
    public IEnumerable<CommandDescriptor> GetCommands(CommandContext context)
    {
      yield return new MoveToPositionDescriptor(context.Position);
    }
  }
}
