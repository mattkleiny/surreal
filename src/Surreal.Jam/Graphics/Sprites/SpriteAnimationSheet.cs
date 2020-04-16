using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Surreal.Graphics.Sprites
{
  public sealed class SpriteAnimationSheet : IReadOnlyList<SpriteAnimation>
  {
    private readonly List<SpriteAnimation> animations = new List<SpriteAnimation>();

    public SpriteAnimationSheet()
      : this(Enumerable.Empty<SpriteAnimation>())
    {
    }

    public SpriteAnimationSheet(IEnumerable<SpriteAnimation> animations)
    {
      this.animations.AddRange(animations);
    }

    public void Add(SpriteAnimation animation)    => animations.Add(animation);
    public void Remove(SpriteAnimation animation) => animations.Remove(animation);

    public int Count => animations.Count;

    public SpriteAnimation this[int index] => animations[index];

    public SpriteAnimation? this[string name]
    {
      get
      {
        for (var i = 0; i < animations.Count; i++)
        {
          var animation = animations[i];

          if (name.Equals(animation.Name, StringComparison.OrdinalIgnoreCase))
          {
            return animation;
          }
        }

        return null;
      }
    }

    public List<SpriteAnimation>.Enumerator                   GetEnumerator() => animations.GetEnumerator();
    IEnumerator<SpriteAnimation> IEnumerable<SpriteAnimation>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.                                  GetEnumerator() => GetEnumerator();
  }
}