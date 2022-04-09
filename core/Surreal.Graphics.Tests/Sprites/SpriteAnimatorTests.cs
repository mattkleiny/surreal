using Surreal.Graphics.Images.Sprites;
using Surreal.Graphics.Textures;
using Surreal.Timing;

namespace Surreal.Graphics.Sprites;

public class SpriteAnimatorTests
{
  [Test, AutoFixture]
  public void it_should_animate_over_time(ISpriteRenderer renderer, TextureRegion region1, TextureRegion region2)
  {
    var animation = new SpriteAnimation
    {
      Name = "Test Animation",
      Flags = SpriteAnimationFlags.Looping,
      Frames = new[]
      {
        new SpriteFrame(Sprite.Create(region1), 0.5f.Seconds()),
        new SpriteFrame(Sprite.Create(region2), 0.5f.Seconds()),
      }
    };

    var animator = new SpriteAnimator(renderer, animation);

    renderer.ClearReceivedCalls();

    animator.Update(0.5f.Seconds());
    animator.Update(0.5f.Seconds());
    animator.Update(0.5f.Seconds());
    animator.Update(0.5f.Seconds());

    renderer.Received(3).Sprite = Arg.Any<Sprite>();
  }
}
