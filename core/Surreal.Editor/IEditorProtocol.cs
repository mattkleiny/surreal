using System.Reactive.Subjects;
using Surreal.Graphics;
using Surreal.Graphics.Textures;
using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;
using Surreal.Mathematics;
using Surreal.Memory;
using Surreal.Timing;

namespace Surreal;

public interface IEditorProtocol
{
  IObservable<FrameEvent>       FrameEvents       { get; }
  IObservable<FrameBufferEvent> FrameBufferEvents { get; }
  IObservable<KeyboardEvent>    KeyboardEvents    { get; }
  IObservable<MouseEvent>       MouseEvents       { get; }
}

public record struct FrameEvent(TimeDelta DeltaTime);
public record struct FrameBufferEvent(Memory<Color32> Pixels);
public record struct KeyboardEvent(Key Key, bool IsPressed);
public record struct MouseEvent(Vector2 Position, MouseButton Button, bool IsPressed);

public sealed class EditorProtocol : IEditorProtocol, IDisposable
{
  private readonly Subject<FrameEvent> frameEvents = new();
  private readonly Subject<FrameBufferEvent> frameBufferEvents = new();
  private readonly Subject<KeyboardEvent> keyboardEvents = new();
  private readonly Subject<MouseEvent> mouseEvents = new();
  private readonly IDisposableBuffer<Color32> pixels;

  private readonly IGraphicsServer graphics;
  private readonly IKeyboardDevice keyboard;
  private readonly IMouseDevice mouse;
  private readonly RenderTarget renderTarget;

  public IObservable<FrameEvent>       FrameEvents       => frameEvents;
  public IObservable<FrameBufferEvent> FrameBufferEvents => frameBufferEvents;
  public IObservable<KeyboardEvent>    KeyboardEvents    => keyboardEvents;
  public IObservable<MouseEvent>       MouseEvents       => mouseEvents;

  public EditorProtocol(IGraphicsServer graphics, IKeyboardDevice keyboard, IMouseDevice mouse)
  {
    this.graphics = graphics;
    this.keyboard = keyboard;
    this.mouse    = mouse;

    var descriptor = new RenderTargetDescriptor(1920, 1080, TextureFormat.Rgba8888);

    renderTarget = new RenderTarget(graphics, descriptor);
    pixels       = Buffers.AllocateNative<Color32>(descriptor.Width * descriptor.Height);
  }

  public void OnBeginFrame(TimeDelta delaTime)
  {
    frameEvents.OnNext(new FrameEvent(delaTime));
  }

  public void OnInput(TimeDelta deltaTime)
  {
    if (keyboard.IsKeyPressed(Key.Escape))
    {
      keyboardEvents.OnNext(new KeyboardEvent(Key.Escape, true));
    }

    if (mouse.IsButtonPressed(MouseButton.Left))
    {
      mouseEvents.OnNext(new MouseEvent(mouse.Position, MouseButton.Left, true));
    }

    renderTarget.Activate();
  }

  public void OnEndFrame(TimeDelta deltaTime)
  {
    renderTarget.ColorAttachment.ReadPixels(pixels.Span);
    renderTarget.Deactivate();

    frameBufferEvents.OnNext(new FrameBufferEvent(pixels.Memory));
  }

  public void Dispose()
  {
    renderTarget.Dispose();
    pixels.Dispose();
  }
}
