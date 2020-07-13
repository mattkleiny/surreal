using System.Collections;
using System.Diagnostics;

namespace Surreal.Collections {
  [DebuggerDisplay("BitArray with {array.Length} elements")]
  public sealed class GrowingBitArray {
    private readonly BitArray array;

    public GrowingBitArray(int initialSize = 32) {
      array = new BitArray(initialSize);
    }

    public bool this[int index] {
      get => index < array.Length && array[index];
      set {
        if (index >= array.Length) Grow();
        array[index] = value;
      }
    }

    public void SetAll(bool value) {
      array.SetAll(value);
    }

    private void Grow()                => Grow((int) (array.Length * 1.5) + 1);
    private void Grow(int newCapacity) => array.Length = newCapacity;

    private bool Equals(GrowingBitArray other) {
      return array.Equals(other.array);
    }

    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;

      return obj is GrowingBitArray bitArray && Equals(bitArray);
    }

    public override int GetHashCode() {
      return array.GetHashCode();
    }
  }
}