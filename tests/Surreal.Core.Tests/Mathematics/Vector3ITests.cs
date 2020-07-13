using Surreal.Mathematics;
using Xunit;

namespace Surreal.Core.Mathematics {
  public class Vector3ITests {
    [Fact]
    public void it_should_add_scalars() => Assert.Equal(MathF.P(10, 10, 10), MathF.P(5, 5, 5) + 5);

    [Fact]
    public void it_should_subtract_scalars() => Assert.Equal(MathF.P(0, 0, 0), MathF.P(5, 5, 5) - 5);

    [Fact]
    public void it_should_multiply_scalars() => Assert.Equal(MathF.P(25, 25, 25), MathF.P(5, 5, 5) * 5);

    [Fact]
    public void it_should_divide_scalars() => Assert.Equal(MathF.P(1, 1, 1), MathF.P(5, 5, 5) / 5);

    [Fact]
    public void it_should_add_points() => Assert.Equal(MathF.P(10, 10, 10), MathF.P(5, 5, 5) + MathF.P(5, 5, 5));

    [Fact]
    public void it_should_subtract_points() => Assert.Equal(MathF.P(0, 0, 0), MathF.P(5, 5, 5) - MathF.P(5, 5, 5));

    [Fact]
    public void it_should_multiply_points() => Assert.Equal(MathF.P(25, 25, 25), MathF.P(5, 5, 5) * MathF.P(5, 5, 5));

    [Fact]
    public void it_should_divide_points() => Assert.Equal(MathF.P(1, 1, 1), MathF.P(5, 5, 5) / MathF.P(5, 5, 5));

    [Fact]
    public void it_should_format_to_string() => Assert.Equal("<73 37 14>", MathF.P(73, 37, 14).ToString());
  }
}