using Surreal.Compute;
using Surreal.Compute.Execution;
using Surreal.Compute.Memory;
using Surreal.Internal.Compute.Resources;

namespace Surreal.Internal.Compute;

internal sealed class OpenTkComputeDevice : IComputeDevice
{
	public ComputeBuffer<T> CreateBuffer<T>()
		where T : unmanaged
	{
		return new OpenTkComputeBuffer<T>();
	}

	public ComputeProgram CreateProgram(ReadOnlySpan<byte> raw)
	{
		return new OpenTkComputeProgram(raw);
	}
}
