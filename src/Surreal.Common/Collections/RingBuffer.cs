using System.Collections;

namespace Surreal.Collections;

/// <summary>A lightweight circular/ring buffer with fixed capacity.</summary>
public sealed class RingBuffer<T> : IEnumerable<T>
{
	private T[] elements;
	private int writePos;

	public RingBuffer(int capacity)
	{
		elements = new T[capacity];
		writePos = 0;
		Count = 0;
	}

	public int Count { get; private set; }
	public int Capacity => elements.Length;

	public ref T this[int index] => ref elements[index];
	public ref T Last => ref elements[Math.Max(writePos - 1, 0)];

	public void Add(T element)
	{
		elements[writePos++] = element;

		if (writePos >= Capacity) writePos = 0; // wrap around ring end
		if (Count < Capacity) Count++; // track occupied slots
	}

	public void Clear()
	{
		for (var i = 0; i < elements.Length; i++)
		{
			elements[i] = default!; // help the GC
		}

		writePos = 0;
		Count = 0;
	}

	public void Resize(int size)
	{
		if (size < elements.Length)
		{
			writePos = Math.Max(writePos, size - 1);
		}

		Array.Resize(ref elements, size);
	}

	public Enumerator GetEnumerator() => new(this);
	IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public struct Enumerator : IEnumerator<T>
	{
		private readonly RingBuffer<T> buffer;
		private int currentPos;
		private int touched;

		public Enumerator(RingBuffer<T> buffer)
			: this()
		{
			this.buffer = buffer;
			Reset();
		}

		public ref T Current => ref buffer.elements[currentPos];
		T IEnumerator<T>.Current => buffer.elements[currentPos];
		object IEnumerator.Current => Current!;

		public bool MoveNext()
		{
			// wrap around the start of the buffer, iterating backwards
			if (--currentPos < 0) currentPos = buffer.Capacity - 1;
			return touched++ < buffer.Count;
		}

		public void Reset()
		{
			touched = 0;
			currentPos = buffer.writePos;
		}

		public void Dispose()
		{
		}
	}
}

public static class RingBufferExtensions
{
	public static float FastAverage(this RingBuffer<float> samples)
	{
		return samples.FastSum() / samples.Count;
	}

	public static float FastSum(this RingBuffer<float> samples)
	{
		float total = 0;

		foreach (var sample in samples)
		{
			total += sample;
		}

		return total;
	}

	public static TimeSpan FastAverage(this RingBuffer<TimeSpan> samples)
	{
		var averageTicks = FastSum(samples).Ticks / samples.Count;

		return TimeSpan.FromTicks(averageTicks);
	}

	public static TimeSpan FastSum(this RingBuffer<TimeSpan> samples)
	{
		var total = TimeSpan.Zero;

		foreach (var sample in samples)
		{
			total += sample;
		}

		return total;
	}

	public static TimeSpan FastMax(this RingBuffer<TimeSpan> samples)
	{
		var result = TimeSpan.MinValue;

		foreach (var sample in samples)
		{
			if (sample > result)
			{
				result = sample;
			}
		}

		return result;
	}

	public static TimeSpan FastMin(this RingBuffer<TimeSpan> samples)
	{
		var result = TimeSpan.MaxValue;

		foreach (var sample in samples)
		{
			if (sample < result)
			{
				result = sample;
			}
		}

		return result;
	}
}
