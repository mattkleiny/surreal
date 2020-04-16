using Surreal.Graphics;
using Surreal.Graphics.SPI.Rasterization;

namespace Surreal.Platform.Internal.Graphics
{
  internal sealed class HeadlessRasterizerState : IRasterizerState
  {
    public Viewport Viewport              { get; set; }
    public bool     IsDepthTestingEnabled { get; set; }
    public bool     IsBlendingEnabled     { get; set; }
  }
}
