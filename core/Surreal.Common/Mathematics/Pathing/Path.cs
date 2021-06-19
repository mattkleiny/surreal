using System.Collections;
using System.Collections.Generic;
using Surreal.Collections;

namespace Surreal.Mathematics.Pathing
{
  public readonly struct Path<T> : IEnumerable<T>
  {
    public static Path<T> Empty => default;

    private readonly ReadOnlySlice<T> steps;

    public Path(ReadOnlySlice<T> steps)
    {
      this.steps = steps;
    }

    public T this[int index] => steps[index];

    public int  Length  => steps.Length;
    public bool IsValid => Length > 0;

    public T Start => this[0];
    public T Goal  => this[Length - 1];

    public ReadOnlySlice<T> Steps => steps;

    public Enumerator             GetEnumerator() => new(this);
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();

    public static implicit operator ReadOnlySlice<T>(Path<T> path) => path.steps;

    public struct Enumerator : IEnumerator<T>
    {
      private readonly Path<T> path;
      private          int     index;

      public Enumerator(Path<T> path)
          : this()
      {
        this.path = path;
        index     = -1;
      }

      public bool    IsValid => index < path.Length && path.IsValid;
      public Path<T> Path    => path;

      public T           Current => path[index];
      object IEnumerator.Current => Current!;

      public bool MoveNext()
      {
        index++;
        return IsValid;
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
}