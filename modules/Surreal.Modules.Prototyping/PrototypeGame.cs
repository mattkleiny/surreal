using Surreal.Audio;
using Surreal.Graphics;
using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;

namespace Surreal;

/// <summary>Base class for any <see cref="Game"/> that uses rapid prototyping services.</summary>
public abstract class PrototypeGame : Game
{
  public IAudioServer    AudioServer    => Services.GetRequiredService<IAudioServer>();
  public IGraphicsServer GraphicsServer => Services.GetRequiredService<IGraphicsServer>();
  public IKeyboardDevice Keyboard       => Services.GetRequiredService<IKeyboardDevice>();
  public IMouseDevice    Mouse          => Services.GetRequiredService<IMouseDevice>();

  protected override void OnInitialize()
  {
    base.OnInitialize();

    OnResized(Host.Width, Host.Height); // initial resize
  }

  protected override void OnResized(int width, int height)
  {
    base.OnResized(width, height);

    if (Services.TryGetService(out IGraphicsServer graphicsServer))
    {
      graphicsServer.SetViewportSize(new Viewport(0, 0, width, height));
    }
  }
}
