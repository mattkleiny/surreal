using Surreal.Combat;

namespace Surreal;

public class DamageTests
{
  [SetUp]
  public void Setup()
  {
    Damage.Calculation = null;
  }

  [Test, AutoFixture]
  public void it_should_calculate_damage_without_callback(int amount)
  {
    var damage = Damage.Calculate(amount, DamageType.Standard);

    Assert.AreEqual(amount, damage.Amount);
    Assert.AreEqual(DamageType.Standard, damage.Type);
  }

  [Test, AutoFixture]
  public void it_should_calculate_damage_via_callback(int amount, DamageType type)
  {
    Damage.Calculation += DamageCalculations.Multiplicative(2);
    Damage.Calculation += DamageCalculations.ReplaceWithType(type);

    var damage = Damage.Calculate(amount, type);

    Assert.AreEqual(amount * 2, damage.Amount);
    Assert.AreEqual(type, damage.Type);
  }
}
