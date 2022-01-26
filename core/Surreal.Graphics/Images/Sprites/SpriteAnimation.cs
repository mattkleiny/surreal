using Surreal.Collections;

namespace Surreal.Graphics.Images.Sprites;

/// <summary>Flags for a <see cref="SpriteAnimation"/>.</summary>
[Flags]
public enum SpriteAnimationFlags
{
  None       = 0,
  Looping    = 1 << 0,
  Everything = ~0
}

/// <summary>A single frame of a <see cref="SpriteAnimation"/>.</summary>
public readonly record struct SpriteFrame(Sprite Sprite, TimeSpan Duration);

/// <summary>An animation of multiple <see cref="SpriteFrame"/>s.</summary>
public sealed record SpriteAnimation
{
  public string               Name   { get; init; } = string.Empty;
  public SpriteAnimationFlags Flags  { get; init; } = SpriteAnimationFlags.None;
  public SpriteFrame[]        Frames { get; init; } = Array.Empty<SpriteFrame>();

  public SpriteFrame this[Index index]
  {
    get => Frames[index];
  }

  public SpriteAnimationSlice this[Range range]
  {
    get
    {
      var (offset, length) = range.GetOffsetAndLength(Frames.Length);

      return new SpriteAnimationSlice(this, offset, length);
    }
  }
}

/// <summary>A slice of a <see cref="SpriteAnimation"/>.</summary>
public readonly record struct SpriteAnimationSlice(SpriteAnimation Animation, int Offset, int Length) : IEnumerable<SpriteFrame>
{
  public bool IsLooping => Animation.Flags.HasFlagFast(SpriteAnimationFlags.Everything);

  public SpriteFrame this[Index index]
  {
    get => Animation[Offset + index.GetOffset(Length)];
  }

  public SpriteAnimationSlice this[Range range]
  {
    get
    {
      var (offset, length) = range.GetOffsetAndLength(Length);

      return new SpriteAnimationSlice(Animation, Offset + offset, length);
    }
  }

  public Enumerator                                 GetEnumerator() => new(this);
  IEnumerator<SpriteFrame> IEnumerable<SpriteFrame>.GetEnumerator() => GetEnumerator();
  IEnumerator IEnumerable.                          GetEnumerator() => GetEnumerator();

  public static implicit operator SpriteAnimationSlice(SpriteAnimation animation) => animation[Range.All];

  /// <summary>An enumerator for the <see cref="SpriteAnimationSlice"/>.</summary>
  public struct Enumerator : IEnumerator<SpriteFrame>
  {
    private readonly SpriteAnimationSlice slice;
    private          int                  index;

    public Enumerator(SpriteAnimationSlice slice)
    {
      this.slice = slice;
      index      = -1;
    }

    public SpriteFrame Current => slice[index];
    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
      return ++index < slice.Length;
    }

    public void Reset()
    {
      index = -1;
    }

    public void Dispose()
    {
      // no-op
    }
  }
}
