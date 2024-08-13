namespace Surreal.Mathematics;

public class AngleTests
{
  [Test]
  public void it_should_lerp_between_angles()
  {
    var a = Angle.FromDegrees(180);
    var b = Angle.FromRadians(2f * MathF.PI);

    var c = MathE.Lerp(a, b, 0.5f);

    c.Degrees.Should().Be(270);
    c.Radians.Should().Be(MathF.PI * 1.5f);
  }
}
