using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Surreal.Selection;

/// <summary>A provider of selected <see cref="ISelectableUnit"/>s.</summary>
public interface IUnitSelectionProvider
{
  IEnumerable<ISelectableUnit> SelectedUnits { get; }
}

/// <summary>The default <see cref="IUnitSelectionProvider"/>, a locally managed collection of units.</summary>
public sealed class UnitSelectionManager : IUnitSelectionProvider
{
  private readonly ObservableCollection<ISelectableUnit> selectedUnits = new();

  public UnitSelectionManager()
  {
    selectedUnits.CollectionChanged += OnCollectionChanged;
  }

  public event Action<ISelectableUnit>? UnitSelected;
  public event Action<ISelectableUnit>? UnitDeselected;

  public IEnumerable<ISelectableUnit> SelectedUnits => selectedUnits;

  public void AddToSelection(ISelectableUnit unit) => selectedUnits.Add(unit);
  public void RemoveFromSelection(ISelectableUnit unit) => selectedUnits.Remove(unit);

  private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs eventArgs)
  {
    if (eventArgs.Action == NotifyCollectionChangedAction.Add)
    {
      OnUnitsSelected(eventArgs);
    }
    else if (eventArgs.Action == NotifyCollectionChangedAction.Remove)
    {
      OnUnitsDeselected(eventArgs);
    }
    else if (eventArgs.Action == NotifyCollectionChangedAction.Replace)
    {
      OnUnitsDeselected(eventArgs);
      OnUnitsSelected(eventArgs);
    }
    else if (eventArgs.Action == NotifyCollectionChangedAction.Reset)
    {
      OnUnitsDeselected(eventArgs);
    }
  }

  private void OnUnitsSelected(NotifyCollectionChangedEventArgs eventArgs)
  {
    foreach (ISelectableUnit unit in eventArgs.NewItems!)
    {
      UnitSelected?.Invoke(unit);
    }
  }

  private void OnUnitsDeselected(NotifyCollectionChangedEventArgs eventArgs)
  {
    foreach (ISelectableUnit unit in eventArgs.OldItems!)
    {
      UnitDeselected?.Invoke(unit);
    }
  }
}
