using Avventura.Graphics.Passes;
using Surreal.Assets;
using Surreal.Graphics;
using Surreal.Graphics.Rendering;
using Surreal.Graphics.Rendering.Culling;
using Surreal.Graphics.Rendering.PostProcessing;

namespace Avventura.Graphics {
  public sealed class DedicatedRenderer : ForwardRenderer {
    public PalettePass  PalettePass  { get; } = new PalettePass();
    public LightingPass LightingPass { get; } = new LightingPass();

    public DesaturationEffect   DesaturationEffect { get; } = new DesaturationEffect();
    public DistortionEffect     DistortionEffect   { get; } = new DistortionEffect();
    public FastAberrationEffect AberrationEffect   { get; } = new FastAberrationEffect();
    public FastBloomEffect      BloomEffect        { get; } = new FastBloomEffect();

    public DedicatedRenderer(
        IGraphicsDevice device,
        ICullingStrategy cullingStrategy,
        in FrameBufferDescriptor colorDescriptor,
        in FrameBufferDescriptor depthDescriptor,
        IAssetResolver assets)
        : base(device, cullingStrategy, colorDescriptor, depthDescriptor, assets) {
      Add(PalettePass);
      Add(LightingPass);

      PostProcessingPass.Add(DesaturationEffect);
      PostProcessingPass.Add(DistortionEffect);
      PostProcessingPass.Add(AberrationEffect);
      PostProcessingPass.Add(BloomEffect);
    }
  }
}