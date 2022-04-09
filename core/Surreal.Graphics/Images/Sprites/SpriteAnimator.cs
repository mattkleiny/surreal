using Surreal.Timing;

namespace Surreal.Graphics.Images.Sprites;

/// <summary>Animates sprites over by delegating to a <see cref="ISpriteRenderer"/>.</summary>
public sealed class SpriteAnimator
{
  private readonly ISpriteRenderer renderer;
  private readonly SpriteAnimationSlice animation;

  private IntervalTimer frameTimer;
  private int frameIndex = 0;

  public SpriteAnimator(ISpriteRenderer renderer, SpriteAnimationSlice animation)
  {
    this.renderer = renderer;
    this.animation = animation;

    if (animation.Length > 0)
    {
      renderer.Sprite = animation[0].Sprite;
      frameTimer = new IntervalTimer(animation[0].Duration);
    }
  }

  public float Speed     { get; set; }         = 1.0f;
  public bool  IsPlaying { get; private set; } = true;

  public void Play() => IsPlaying = true;
  public void Stop() => IsPlaying = false;

  public void Update(DeltaTime deltaTime)
  {
    if (IsPlaying && frameTimer.Tick(deltaTime * Speed))
    {
      if (++frameIndex >= animation.Length)
      {
        if (animation.IsLooping)
        {
          frameIndex = 0;
        }
        else
        {
          IsPlaying = false;

          renderer.Sprite = animation[^1].Sprite;
          frameIndex = animation.Length - 1;
        }
      }

      if (frameIndex < animation.Length)
      {
        var (sprite, duration) = animation[frameIndex];

        renderer.Sprite = sprite;
        frameTimer = new IntervalTimer(duration);
      }
    }
  }
}
