namespace Surreal.Combat;

public class DamageTypeTests
{
  [Test]
  public void it_should_pre_compute_damage_type_hash()
  {
    var acid1 = new DamageType("ACID");
    var acid2 = new DamageType("Acid");
    var acid3 = new DamageType("acid");

    Assert.AreEqual(acid1, acid2);
    Assert.AreEqual(acid1, acid3);
  }
}
