using NUnit.Framework;

namespace Surreal;

public class DamageTests
{
  private static DamageType Acid { get; } = new("Acid");

  [SetUp]
  public void Setup()
  {
    Damage.Calculation = null;
  }

  [Test]
  public void it_should_calculate_damage_without_callback()
  {
    var damage = Damage.Calculate(5, DamageType.Standard);

    Assert.AreEqual(5, damage.Amount);
    Assert.AreEqual(DamageType.Standard, damage.Type);
  }

  [Test]
  public void it_should_calculate_damage_via_callback()
  {
    Damage.Calculation += DamageCalculations.Combine(
      DamageCalculations.Multiplicative(2),
      DamageCalculations.ReplaceWithType(Acid)
    );

    var damage = Damage.Calculate(5, DamageType.Standard);

    Assert.AreEqual(10, damage.Amount);
    Assert.AreEqual(Acid, damage.Type);
  }
}
