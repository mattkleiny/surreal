using NUnit.Framework;

namespace Surreal.Collections;

#pragma warning disable S2344

public class EnumExtensionTests
{
  [Test]
  public void it_should_cast_to_int()
  {
    Assert.AreEqual(0, TestEnum.Zero.AsInt());
    Assert.AreEqual(1, TestEnum.One.AsInt());
    Assert.AreEqual(2, TestEnum.Two.AsInt());
    Assert.AreEqual(3, TestEnum.Three.AsInt());
    Assert.AreEqual(4, TestEnum.Four.AsInt());
  }

  [Test]
  public void it_should_cast_from_int()
  {
    Assert.AreEqual(TestEnum.Zero, 0.AsEnum<TestEnum>());
    Assert.AreEqual(TestEnum.One, 1.AsEnum<TestEnum>());
    Assert.AreEqual(TestEnum.Two, 2.AsEnum<TestEnum>());
    Assert.AreEqual(TestEnum.Three, 3.AsEnum<TestEnum>());
    Assert.AreEqual(TestEnum.Four, 4.AsEnum<TestEnum>());
  }

  [Test]
  public void it_should_compare_as_int()
  {
    Assert.IsTrue(TestEnum.Four.EqualsFast(TestEnum.Four));
  }

  private enum TestEnum
  {
    Zero,
    One,
    Two,
    Three,
    Four
  }
}
