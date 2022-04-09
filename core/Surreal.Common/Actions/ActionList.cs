namespace Surreal.Actions;

/// <summary>A collection <see cref="IAction"/>s that can be operated on in bulk.</summary>
public sealed class ActionList : IEnumerable<IAction>, IAction
{
  private readonly List<IAction> actions = new();

  public void Add(IAction action) => actions.Add(action);
  public bool Remove(IAction action) => actions.Remove(action);

  public async ValueTask ExecuteAsync(ActionContext context)
  {
    foreach (var action in actions)
    {
      await action.ExecuteAsync(context);
    }
  }

  public List<IAction>.Enumerator GetEnumerator() => actions.GetEnumerator();
  IEnumerator<IAction> IEnumerable<IAction>.GetEnumerator() => GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
