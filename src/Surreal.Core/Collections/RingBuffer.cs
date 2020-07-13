using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Surreal.Collections {
  [DebuggerDisplay("RingBuffer {Count}/{Capacity}")]
  public sealed class RingBuffer<T> : IEnumerable<T> {
    private readonly T[] elements;
    private          int writePos;

    public RingBuffer(int capacity) {
      elements = new T[capacity];
    }

    public int Count    { get; private set; }
    public int Capacity => elements.Length;

    public ref T this[int index] => ref elements[index];

    public void Add(T element) {
      elements[writePos++] = element;

      if (writePos >= Capacity) writePos = 0; // wrap around ring end
      if (Count    < Capacity) Count++;       // track occupied slots
    }

    public void Clear() {
      for (var i = 0; i < elements.Length; i++) {
        elements[i] = default!; // help the GC
      }

      writePos = 0;
      Count    = 0;
    }

    public Enumerator             GetEnumerator() => new Enumerator(this);
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.      GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<T> {
      private readonly RingBuffer<T> buffer;
      private          int           currentPos;
      private          int           touched;

      public Enumerator(RingBuffer<T> buffer)
          : this() {
        this.buffer = buffer;
        Reset();
      }

      public T           Current => buffer.elements[currentPos];
      object IEnumerator.Current => Current!;

      public bool MoveNext() {
        // wrap around the start of the buffer, iterating backwards
        if (--currentPos < 0) currentPos = buffer.Capacity - 1;
        return touched++ < buffer.Count;
      }

      public void Reset() {
        touched    = 0;
        currentPos = buffer.writePos;
      }

      public void Dispose() {
      }
    }
  }

  public static class RingBufferExtensions {
    public static TimeSpan FastSum(this RingBuffer<TimeSpan> samples) {
      long totalTicks = 0;

      foreach (var sample in samples) {
        totalTicks += sample.Ticks;
      }

      return TimeSpan.FromTicks(totalTicks);
    }

    public static TimeSpan FastAverage(this RingBuffer<TimeSpan> samples) {
      var average = samples.FastSum().Ticks / samples.Count;

      return TimeSpan.FromTicks(average);
    }

    public static TimeSpan FastMax(this RingBuffer<TimeSpan> samples) {
      var result = TimeSpan.MinValue;

      foreach (var sample in samples) {
        if (sample > result) {
          result = sample;
        }
      }

      return result;
    }

    public static TimeSpan FastMin(this RingBuffer<TimeSpan> samples) {
      var result = TimeSpan.MaxValue;

      foreach (var sample in samples) {
        if (sample < result) {
          result = sample;
        }
      }

      return result;
    }
  }
}