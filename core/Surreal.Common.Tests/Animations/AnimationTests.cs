using Surreal.Mathematics;
using Surreal.Timing;

namespace Surreal.Animations;

public class AnimationTests
{
  [Test]
  public void it_should_advance_and_rewind_values_over_time()
  {
    var instance = new TestObject();

    var animation = new Animation
    {
      Tracks =
      {
        AnimationTrack.Create(instance, _ => _.Color) with
        {
          KeyFrames = new[]
          {
            new KeyFrame<Color>(0f, Color.Black),
            new KeyFrame<Color>(1f, Color.White),
          },
        },
        AnimationTrack.Create(instance, _ => _.Depth) with
        {
          KeyFrames = new[]
          {
            new KeyFrame<float>(0f, 0f),
            new KeyFrame<float>(1f, 1f),
          },
        },
      },
    };

    animation.Advance(1.Seconds());

    instance.Color.Should().Be(Color.White);
    instance.Depth.Should().Be(1f);

    animation.Rewind(1.Seconds());

    instance.Color.Should().Be(Color.Black);
    instance.Depth.Should().Be(0f);
  }

  private sealed record TestObject
  {
    public Color Color { get; set; } = Color.Black;
    public float Depth { get; set; } = 0f;
  }
}
