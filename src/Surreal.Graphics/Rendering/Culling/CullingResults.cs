using System;

namespace Surreal.Graphics.Rendering.Culling
{
  public readonly struct CullingResults
  {
    public ReadOnlySpan<CulledRenderer> VisibleRenderers     => throw new NotImplementedException();
    public ReadOnlySpan<CulledRenderer> OpaqueRenderers      => throw new NotImplementedException();
    public ReadOnlySpan<CulledRenderer> TransparentRenderers => throw new NotImplementedException();
  }
}