using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Surreal;

public interface ISelectableUnit
{
}

public interface IUnitSelectionProvider
{
  IEnumerable<ISelectableUnit> SelectedUnits { get; }
}

public sealed class UnitSelectionProvider : IUnitSelectionProvider
{
  private readonly ObservableCollection<ISelectableUnit> selectedUnits = new();

  public UnitSelectionProvider()
  {
    selectedUnits.CollectionChanged += OnCollectionChanged;
  }

  public event Action<ISelectableUnit>? UnitSelected;
  public event Action<ISelectableUnit>? UnitDeselected;

  public IEnumerable<ISelectableUnit> SelectedUnits => selectedUnits;

  public void AddToSelection(ISelectableUnit unit)      => selectedUnits.Add(unit);
  public void RemoveFromSelection(ISelectableUnit unit) => selectedUnits.Remove(unit);

  private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs eventArgs)
  {
    if (eventArgs.Action == NotifyCollectionChangedAction.Add)
    {
      foreach (ISelectableUnit unit in eventArgs.NewItems!)
      {
        UnitSelected?.Invoke(unit);
      }
    }
    else if (eventArgs.Action == NotifyCollectionChangedAction.Remove)
    {
      foreach (ISelectableUnit unit in eventArgs.OldItems!)
      {
        UnitDeselected?.Invoke(unit);
      }
    }
    else if (eventArgs.Action == NotifyCollectionChangedAction.Replace)
    {
      foreach (ISelectableUnit unit in eventArgs.OldItems!)
      {
        UnitDeselected?.Invoke(unit);
      }

      foreach (ISelectableUnit unit in eventArgs.NewItems!)
      {
        UnitSelected?.Invoke(unit);
      }
    }
    else if (eventArgs.Action == NotifyCollectionChangedAction.Reset)
    {
      foreach (ISelectableUnit item in eventArgs.OldItems!)
      {
        UnitDeselected?.Invoke(item);
      }
    }
  }
}
