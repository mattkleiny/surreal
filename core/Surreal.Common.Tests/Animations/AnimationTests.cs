using Surreal.Colors;

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
        AnimationTrack.Create(instance, it => it.Color) with
        {
          KeyFrames =
          [
            new KeyFrame<Color>(0f, Color.Black),
            new KeyFrame<Color>(1f, Color.White)
          ],
        },
        AnimationTrack.Create(instance, it => it.Depth) with
        {
          KeyFrames =
          [
            new KeyFrame<float>(0f, 0f),
            new KeyFrame<float>(1f, 1f)
          ],
        },
      },
    };

    animation.Advance(TimeSpan.FromSeconds(1f));

    Assert.AreEqual(Color.White, instance.Color);
    Assert.AreEqual(1f, instance.Depth);

    animation.Rewind(TimeSpan.FromSeconds(1f));

    Assert.AreEqual(Color.Black, instance.Color);
    Assert.AreEqual(0f, instance.Depth);
  }

  private sealed record TestObject
  {
    public Color Color { get; set; } = Color.Black;
    public float Depth { get; set; } = 0f;
  }
}
