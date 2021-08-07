using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Surreal.IO;

namespace Surreal.Collections
{
  public struct BitArray32 : IEquatable<BitArray32>, IEnumerable<bool>, IBinarySerializable
  {
    public static BitArray32 Empty => default;

    private uint bits;

    public BitArray32(uint bits)
    {
      this.bits = bits;
    }

    public uint Bits   => bits;
    public int  Length => 32;

    public bool this[int index]
    {
      get
      {
        Debug.Assert(index >= 0, "index >= 0");
        Debug.Assert(index < Length, "index < Length");

        var mask = 1u << index;

        return (bits & mask) == mask;
      }
      set
      {
        Debug.Assert(index >= 0, "index >= 0");
        Debug.Assert(index < Length, "index < Length");

        var mask = 1u << index;

        if (value)
          bits |= mask;
        else
          bits &= ~mask;
      }
    }

    public override bool Equals(object? obj)    => obj is BitArray32 array && bits == array.bits;
    public          bool Equals(BitArray32 arr) => bits == arr.bits;

    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
    public override int GetHashCode() => bits.GetHashCode();

    public override string ToString()
    {
      var builder = new StringBuilder();

      for (var i = 0; i < Length; i++)
      {
        builder.Append(this[i] ? "1" : "0");
      }

      return builder.ToString();
    }

    public Enumerator                   GetEnumerator() => new(bits);
    IEnumerator<bool> IEnumerable<bool>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.            GetEnumerator() => GetEnumerator();

    public static bool operator ==(BitArray32 left, BitArray32 right) => left.Equals(right);
    public static bool operator !=(BitArray32 left, BitArray32 right) => !left.Equals(right);

    void IBinarySerializable.Save(BinaryWriter writer)
    {
      writer.Write(bits);
    }

    void IBinarySerializable.Load(BinaryReader reader)
    {
      bits = reader.ReadUInt32();
    }

    public struct Enumerator : IEnumerator<bool>
    {
      private readonly uint bits;
      private          int  currentIndex;

      public Enumerator(uint bits)
      {
        this.bits = bits;

        currentIndex = -1;
      }

      object IEnumerator.Current => Current;

      public bool Current
      {
        get
        {
          var mask = 1u << currentIndex;

          return (bits & mask) == mask;
        }
      }

      public bool MoveNext()
      {
        currentIndex++;

        return currentIndex < 32;
      }

      public void Reset()
      {
        currentIndex = -1;
      }

      public void Dispose()
      {
      }
    }
  }
}
