using System.Threading.Tasks;
using Surreal.Framework.Parameters;
using Surreal.Framework.Tweening;
using Surreal.Mathematics;
using Surreal.Mathematics.Timing;
using Xunit;

namespace Surreal.Jam.Framework {
  public class ParameterTests {
    private FixedStepClock Clock { get; } = FixedStepClock.CreateDefault();

    [Fact]
    public async Task it_should_tween_values_over_time() {
      var parameter = new AngleParameter(Angle.Zero);

      await parameter.TweenOverTime(Clock, Angle.Zero, Angle.FromDegrees(360), TweenAnimation.Default);
      
      Assert.Equal(360.0f, parameter.Value.Degrees);
    }
  }
}