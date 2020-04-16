using System;
using Surreal.Framework;
using Surreal.Framework.Screens;
using Surreal.Graphics.Cameras;

namespace Surreal.Graphics.Rendering
{
  public sealed class ScreenRenderingPlugin : ScreenPlugin, IDisposable
  {
    public ICamera                Camera   { get; }
    public IRenderer              Renderer { get; }
    public StandardRenderPipeline Pipeline { get; }

    public ScreenRenderingPlugin(ICamera camera, IRenderer renderer)
    {
      Camera   = camera;
      Renderer = renderer;

      Pipeline = new StandardRenderPipeline(Renderer);
    }

    public override void Draw(GameTime time)
    {
      base.Draw(time);

      Pipeline.Render(Camera);
    }

    public void Dispose()
    {
      Pipeline.Dispose();
    }
  }
}