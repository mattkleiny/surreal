namespace Surreal.Collections;

public class EnumExtensionTests
{
  [Test]
  public void it_should_cast_to_int()
  {
    TestEnum.Zero.AsInt().Should().Be(0);
    TestEnum.One.AsInt().Should().Be(1);
    TestEnum.Two.AsInt().Should().Be(2);
    TestEnum.Three.AsInt().Should().Be(3);
    TestEnum.Four.AsInt().Should().Be(4);
  }

  [Test]
  public void it_should_cast_from_int()
  {
    0.AsEnum<TestEnum>().Should().Be(TestEnum.Zero);
    1.AsEnum<TestEnum>().Should().Be(TestEnum.One);
    2.AsEnum<TestEnum>().Should().Be(TestEnum.Two);
    3.AsEnum<TestEnum>().Should().Be(TestEnum.Three);
    4.AsEnum<TestEnum>().Should().Be(TestEnum.Four);
  }

  [Test]
  public void it_should_compare_as_int()
  {
    TestEnum.Four.EqualsFast(TestEnum.Four).Should().BeTrue();
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
