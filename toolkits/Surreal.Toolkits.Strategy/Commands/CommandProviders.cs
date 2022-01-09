using Surreal.Selection;

namespace Surreal.Commands;

/// <summary>A provider for <see cref="ICommand"/>s in some context.</summary>
public interface ICommandProvider
{
  IEnumerable<CommandDescriptor> GetCommands(CommandContext context);
}

/// <summary>Provides <see cref="ICommand"/>s from selected units in a <see cref="IUnitSelectionProvider"/>.</summary>
public sealed class SelectedUnitCommandProvider : ICommandProvider
{
  private readonly IUnitSelectionProvider selectionProvider;

  public SelectedUnitCommandProvider(IUnitSelectionProvider selectionProvider)
  {
    this.selectionProvider = selectionProvider;
  }

  public IEnumerable<CommandDescriptor> GetCommands(CommandContext context)
  {
    return selectionProvider.SelectedUnits.OfType<ICommandProvider>().SelectMany(_ => _.GetCommands(context));
  }
}
