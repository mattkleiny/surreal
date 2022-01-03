namespace Surreal;

public interface ISelectableUnit
{
}

public interface IUnitSelectionManager
{
  event Action SelectionChanged;

  IEnumerable<ISelectableUnit> SelectedUnits { get; }
}

public sealed class UnitSelectionManager : IUnitSelectionManager
{
  public event Action? SelectionChanged;

  public LinkedList<ISelectableUnit> SelectedUnits { get; } = new();

  public void AddToSelection(ISelectableUnit unit)
  {
    SelectedUnits.AddLast(unit);
    SelectionChanged?.Invoke();
  }

  public void RemoveFromSelection(ISelectableUnit unit)
  {
    SelectedUnits.Remove(unit);
    SelectionChanged?.Invoke();
  }

  IEnumerable<ISelectableUnit> IUnitSelectionManager.SelectedUnits => SelectedUnits;
}
