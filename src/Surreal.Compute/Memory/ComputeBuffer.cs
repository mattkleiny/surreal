using Surreal.Memory;

namespace Surreal.Compute.Memory;

/// <summary>A buffer of data on the <see cref="IComputeDevice"/>.</summary>
public abstract class ComputeBuffer : ComputeResource, IHasSizeEstimate
{
	public int Length { get; protected set; }
	public Size Size { get; protected set; }
}

/// <summary>A strongly-typed <see cref="ComputeBuffer"/> of <see cref="T"/>.</summary>
public abstract class ComputeBuffer<T> : ComputeBuffer
	where T : unmanaged
{
	public Memory<T> Read() => Read(Range.All);
	public abstract Memory<T> Read(Range range);
	public abstract void Write(ReadOnlySpan<T> data);
}
