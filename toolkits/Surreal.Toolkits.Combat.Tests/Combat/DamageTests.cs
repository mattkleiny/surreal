namespace Surreal.Combat;

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

    damage.Amount.Should().Be(amount);
    damage.Type.Should().Be(DamageType.Standard);
  }

  [Test, AutoFixture]
  public void it_should_calculate_damage_via_callback(int amount, DamageType type)
  {
    Damage.Calculation += DamageCalculations.Multiplicative(2);
    Damage.Calculation += DamageCalculations.ReplaceWithType(type);

    var damage = Damage.Calculate(amount, type);

    damage.Amount.Should().Be(amount * 2);
    damage.Type.Should().Be(type);
  }
}
