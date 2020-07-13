using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Surreal.Mathematics.Linear;

namespace Surreal.Mathematics {
  public interface INeighbourhood {
    bool     MoveNext(ref int index);
    Vector2I GetOffset(int index);
  }

  public readonly struct MooreNeighbourhood : INeighbourhood {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext(ref int index) {
      return ++index < 9;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2I GetOffset(int index) {
      var y = index / 3 - 1;
      var x = index % 3 - 1;

      return new Vector2I(x, y);
    }
  }

  public readonly struct VonNeumannNeighbourhood : INeighbourhood {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext(ref int index) {
      return ++index < 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2I GetOffset(int index) {
      if (index <= 2) {
        var x = index % 3 - 1;

        return new Vector2I(x, 0);
      }

      var y = index % 3 + (index == 3 ? -1 : 0);

      return new Vector2I(0, y);
    }
  }

  public static class Neighbourhoods {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NeighbourhoodEnumerator<TNeighbourhood> GetNeighbourhood<TNeighbourhood>(this Vector2I center)
        where TNeighbourhood : struct, INeighbourhood {
      return new NeighbourhoodEnumerator<TNeighbourhood>(center, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NeighbourhoodEnumerator<MooreNeighbourhood> GetMooreNeighbourhood(this Vector2I center) {
      return new NeighbourhoodEnumerator<MooreNeighbourhood>(center, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NeighbourhoodEnumerator<VonNeumannNeighbourhood> GetVonNeumannNeighbourhood(this Vector2I center) {
      return new NeighbourhoodEnumerator<VonNeumannNeighbourhood>(center, default);
    }

    public struct NeighbourhoodEnumerator<TNeighbourhood> : IEnumerator<Vector2I>, IEnumerable<Vector2I>
        where TNeighbourhood : struct, INeighbourhood {
      private readonly Vector2I center;

      private TNeighbourhood neighbourhood;
      private int            index;

      public NeighbourhoodEnumerator(Vector2I center, TNeighbourhood neighbourhood) {
        this.center        = center;
        this.neighbourhood = neighbourhood;

        index = 0;

        Reset();
      }

      public bool        MoveNext() => neighbourhood.MoveNext(ref index);
      public void        Reset()    => index = -1;
      public Vector2I    Current    => center + neighbourhood.GetOffset(index);
      object IEnumerator.Current    => Current;

      public void Dispose() {
      }

      public NeighbourhoodEnumerator<TNeighbourhood> GetEnumerator() => this;
      IEnumerator<Vector2I> IEnumerable<Vector2I>.   GetEnumerator() => GetEnumerator();
      IEnumerator IEnumerable.                       GetEnumerator() => GetEnumerator();
    }
  }
}