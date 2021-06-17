using System.Collections;
using System.Collections.Generic;
using Surreal.Mathematics.Linear;

namespace Surreal.Mathematics.Grids {
  public struct GridEnumerator : IEnumerator<Point2>, IEnumerable<Point2> {
    private readonly int stride;
    private readonly int length;
    private          int index;

    public GridEnumerator(int width, int height) {
      stride = width;
      length = width * height;
      index  = -1;
    }

    public Point2 Current {
      get {
        var x = index % stride;
        var y = index / stride;

        return new Point2(x, y);
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
    IEnumerator<Point2> IEnumerable<Point2>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.                    GetEnumerator() => GetEnumerator();
  }
}