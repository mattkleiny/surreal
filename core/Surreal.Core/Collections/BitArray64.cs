using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Surreal.Collections {
  public struct BitArray64 : IEquatable<BitArray64>, IReadOnlyList<bool> {
    public static BitArray64 Empty => default;

    private ulong bits;

    public BitArray64(ulong bits) {
      this.bits = bits;
    }

    public int Length => 64;

    public bool this[int index] {
      get {
        Debug.Assert(index >= 0, "index >= 0");
        Debug.Assert(index < Length, "index < Length");

        var mask = 1ul << index;

        return (bits & mask) == mask;
      }
      set {
        Debug.Assert(index >= 0, "index >= 0");
        Debug.Assert(index < Length, "index < Length");

        var mask = 1ul << index;

        if (value) {
          bits |= mask;
        }
        else {
          bits &= ~mask;
        }
      }
    }

    public void Clear() {
      bits = 0;
    }

    public override bool Equals(object? obj)    => obj is BitArray64 array && bits == array.bits;
    public          bool Equals(BitArray64 arr) => bits == arr.bits;

    public override int GetHashCode() => bits.GetHashCode();

    public override string ToString() {
      var builder = new StringBuilder();

      for (var i = 0; i < Length; i++) {
        builder.Append(this[i] ? "1" : "0");
      }

      return builder.ToString();
    }

    public Enumerator                   GetEnumerator() => new(bits);
    IEnumerator<bool> IEnumerable<bool>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.            GetEnumerator() => GetEnumerator();

    int IReadOnlyCollection<bool>.Count => Length;

    public struct Enumerator : IEnumerator<bool> {
      private readonly ulong bits;
      private          int   currentIndex;

      public Enumerator(ulong bits) {
        this.bits = bits;

        currentIndex = -1;
      }

      object IEnumerator.Current => Current;

      public bool Current {
        get {
          var mask = 1ul << currentIndex;

          return (bits & mask) == mask;
        }
      }

      public bool MoveNext() {
        currentIndex++;

        return currentIndex < 64;
      }

      public void Reset() {
        currentIndex = -1;
      }

      public void Dispose() {
      }
    }
  }
}