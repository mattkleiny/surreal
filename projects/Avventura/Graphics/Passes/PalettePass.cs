using Avventura.Graphics.Palettes;
using Surreal.Graphics.Rendering;

namespace Avventura.Graphics.Passes
{
  public sealed class PalettePass : IRenderingPass
  {
    private ColorPaletteSettings paletteSettings = ColorPaletteSettings.Default;

    public ref ColorPaletteSettings PaletteSettings => ref paletteSettings;

    RenderingStage IRenderingPass.Stage => RenderingStage.BeforeAll;

    public void Render(ref RenderingContext context)
    {
    }
  }
}