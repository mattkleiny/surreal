using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Surreal.Graphics.Sprites {
  public sealed class SpriteAnimation : IReadOnlyList<SpriteFrame> {
    private readonly List<SpriteFrame> frames = new List<SpriteFrame>();

    public SpriteAnimation(string name)
        : this(name, Enumerable.Empty<SpriteFrame>()) {
    }

    public SpriteAnimation(string name, IEnumerable<SpriteFrame> frames) {
      Name = name;

      this.frames.AddRange(frames);
    }

    public string Name            { get; }
    public float  FramesPerSecond { get; set; } = 60;
    public bool   IsLooping       { get; set; }
    public bool   IsCycling       { get; set; }

    public void Add(SpriteFrame frame)    => frames.Add(frame);
    public void Remove(SpriteFrame frame) => frames.Remove(frame);

    public int Count {
      get {
        if (IsCycling) {
          // double the frames, minus the start and the end
          return 2 * frames.Count - 2;
        }

        return frames.Count;
      }
    }

    public SpriteFrame this[int index] {
      get {
        if (IsCycling && index >= frames.Count) {
          // cycle backwards after the end of the array
          return frames[Count - index];
        }

        return frames[index];
      }
    }

    public List<SpriteFrame>.Enumerator               GetEnumerator() => frames.GetEnumerator();
    IEnumerator<SpriteFrame> IEnumerable<SpriteFrame>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.                          GetEnumerator() => GetEnumerator();
  }
}