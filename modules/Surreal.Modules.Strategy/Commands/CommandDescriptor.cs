namespace Surreal.Commands;

/// <summary>A descriptor for a game command.</summary>
public abstract record CommandDescriptor(string Name)
{
  public abstract ICommand CreateCommand();
}
