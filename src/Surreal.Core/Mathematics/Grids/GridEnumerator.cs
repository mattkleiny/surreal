using System.Collections;
using System.Collections.Generic;
using Surreal.Mathematics.Linear;

namespace Surreal.Mathematics.Grids {
  public struct GridEnumerator : IEnumerator<Vector2I>, IEnumerable<Vector2I> {
    private readonly int stride;
    private readonly int length;
    private          int index;

    public GridEnumerator(int width, int height) {
      stride = width;
      length = width * height;
      index  = -1;
    }

    public Vector2I Current {
      get {
        var x = index % stride;
        var y = index / stride;

        return new Vector2I(x, y);
      }
    }

    object IEnumerator.Current => Current;

    public bool MoveNext() {
      return ++index < length;
    }

    public void Reset() {
      index = -1;
    }

    public void Dispose() {
      // no-op
    }

    public GridEnumerator                       GetEnumerator() => this;
    IEnumerator<Vector2I> IEnumerable<Vector2I>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.                    GetEnumerator() => GetEnumerator();
  }
}