using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Surreal.Mathematics;
using Vector = Avalonia.Vector;

namespace Surreal.Controls;

public class GameDisplayViewport : Control, IDisposable
{
  private readonly IEditorProtocol protocol;
  private readonly WriteableBitmap bitmap;

  private IDisposable? subscription;

  public GameDisplayViewport(IEditorProtocol protocol, int width, int height)
  {
    this.protocol = protocol;

    bitmap = new WriteableBitmap(new PixelSize(width, height), Vector.One, PixelFormat.Rgba8888, AlphaFormat.Unpremul);
  }

  protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
  {
    base.OnAttachedToVisualTree(e);

    subscription = protocol.FrameBufferEvents.Subscribe(OnFrameBufferChanged);
  }

  protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
  {
    subscription?.Dispose();

    base.OnDetachedFromVisualTree(e);
  }

  private unsafe void OnFrameBufferChanged(FrameBufferEvent @event)
  {
    using var framebuffer = bitmap.Lock();

    var targetPixels = new Span<Color32>(
      pointer: framebuffer.Address.ToPointer(),
      length: framebuffer.Size.Width * framebuffer.Size.Height
    );

    @event.Pixels.Span.CopyTo(targetPixels);
  }

  public override void Render(DrawingContext context)
  {
    base.Render(context);

    context.DrawImage(bitmap, Bounds);
  }

  public void Dispose()
  {
    bitmap.Dispose();
  }
}
