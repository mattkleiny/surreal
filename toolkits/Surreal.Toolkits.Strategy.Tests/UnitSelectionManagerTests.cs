using NSubstitute;
using NUnit.Framework;

namespace Surreal;

public class UnitSelectionManagerTests
{
  [Test]
  public void it_should_notify_on_newly_selected_units()
  {
    var selectedUnits = new HashSet<ISelectableUnit>();
    var manager       = new UnitSelectionProvider();

    manager.UnitSelected   += unit => selectedUnits.Add(unit);
    manager.UnitDeselected += unit => selectedUnits.Remove(unit);

    var unit1 = Substitute.For<ISelectableUnit>();
    var unit2 = Substitute.For<ISelectableUnit>();
    var unit3 = Substitute.For<ISelectableUnit>();

    manager.AddToSelection(unit1);
    manager.AddToSelection(unit2);
    manager.AddToSelection(unit3);
    manager.RemoveFromSelection(unit2);

    Assert.That(selectedUnits, Is.EquivalentTo(new[] { unit1, unit3 }));
  }
}
