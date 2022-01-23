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

    Assert.AreEqual(Color.White, instance.Color);
    Assert.AreEqual(1f, instance.Depth);

    animation.Rewind(1.Seconds());

    Assert.AreEqual(Color.Black, instance.Color);
    Assert.AreEqual(0f, instance.Depth);
  }

  private sealed record TestObject
  {
    public Color Color { get; set; } = Color.Black;
    public float Depth { get; set; } = 0f;
  }
}
