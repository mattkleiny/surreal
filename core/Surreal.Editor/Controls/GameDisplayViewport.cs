using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Surreal.Mathematics;

namespace Surreal.Controls;

public class GameDisplayViewport : Control, IDisposable
{
  private readonly WriteableBitmap bitmap;

  public GameDisplayViewport()
  {
    bitmap = new WriteableBitmap(
      size: new(1920, 1080),
      dpi: new(96f, 96f),
      format: PixelFormat.Rgba8888,
      alphaFormat: AlphaFormat.Unpremul
    );
  }

  public override void Render(DrawingContext context)
  {
    base.Render(context);

    context.DrawImage(bitmap, Bounds);
  }

  public unsafe void OnFrameBufferChanged(FrameBufferEvent @event)
  {
    using var framebuffer = bitmap.Lock();

    @event.Pixels.Span.CopyTo(new Span<Color32>(
      pointer: framebuffer.Address.ToPointer(),
      length: framebuffer.Size.Width * framebuffer.Size.Height
    ));

    InvalidateVisual();
  }

  public void Dispose()
  {
    bitmap.Dispose();
  }
}
