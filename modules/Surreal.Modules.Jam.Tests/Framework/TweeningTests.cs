using System.Numerics;
using System.Threading.Tasks;
using Surreal.Framework.Parameters;
using Surreal.Framework.Tweening;
using Surreal.Graphics;
using Surreal.Mathematics.Curves;
using Surreal.Mathematics.Timing;
using Xunit;

namespace Surreal.Jam.Framework {
  public class TweeningTests {
    private FixedStepClock Clock { get; } = FixedStepClock.CreateDefault();

    [Fact]
    public async Task it_should_tween_a_simple_parameter_over_time_using_linear_curve() {
      var parameter = new ColorParameter(Color.Black);

      await parameter.TweenOverTime(
          clock: Clock,
          a: Color.Black,
          b: Color.White,
          animation: new TweenAnimation(10.Seconds(), AnimationCurves.Linear)
      );

      Assert.Equal(Color.White, parameter.Value);
    }

    [Fact]
    public async Task it_should_tween_a_simple_parameter_over_time_using_inverse_linear_curve() {
      var parameter = new ColorParameter(Color.Black);

      await parameter.TweenOverTime(
          clock: Clock,
          a: Color.Black,
          b: Color.White,
          animation: new TweenAnimation(10.Seconds(), AnimationCurves.InverseLinear)
      );

      Assert.Equal(Color.Black, parameter.Value);
    }

    [Fact]
    public async Task it_should_tween_a_simple_parameter_over_time_using_quadratic_curve() {
      var parameter = new ColorParameter(Color.Black);

      await parameter.TweenOverTime(
          clock: Clock,
          a: Color.Black,
          b: Color.White,
          animation: new TweenAnimation(
              duration: 10.Seconds(),
              curve: AnimationCurves.PlanarX(
                  new QuadraticBezierCurve(
                      startPoint: Vector2.Zero,
                      controlPoint: new Vector2(0f, 1f),
                      endPoint: Vector2.One
                  )
              )
          )
      );

      Assert.Equal(Color.White, parameter.Value);
    }
  }
}