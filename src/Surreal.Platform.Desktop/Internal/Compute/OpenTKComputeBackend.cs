using System;
using Surreal.Compute.Execution;
using Surreal.Compute.Memory;
using Surreal.Compute.SPI;
using Surreal.Platform.Internal.Compute.Resources;

namespace Surreal.Platform.Internal.Compute
{
  internal sealed class OpenTKComputeBackend : IComputeBackend, IComputeFactory, IDisposable
  {
    public IComputeFactory Factory => this;

    public ComputeBuffer CreateBuffer(int width, int height)
    {
      return new OpenTKComputeBuffer(width, height);
    }

    public ComputeProgram CreateProgram(ReadOnlySpan<byte> raw)
    {
      return new OpenTKComputeProgram(raw);
    }

    public void Dispose()
    {
    }
  }
}