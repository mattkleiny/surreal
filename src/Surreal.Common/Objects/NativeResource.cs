using System;

#pragma warning disable CA1816

namespace Surreal.Objects
{
  /// <summary>A resource with native underlying data that must be finalised.</summary>
  public abstract class NativeResource : Resource
  {
    ~NativeResource()
    {
      Dispose(false);
    }

    protected override void Dispose(bool managed)
    {
      GC.SuppressFinalize(this);

      base.Dispose(managed);
    }
  }
}
