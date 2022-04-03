using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;
using Surreal.Threading;
using Surreal.Timing;

namespace Surreal;

/// <summary>The <see cref="IPlatformHost"/> for <see cref="ConsolePlatform"/>.</summary>
internal sealed class ConsolePlatformHost : IPlatformHost, IServiceModule
{
  private readonly ConsoleConfiguration configuration;

  public ConsolePlatformHost(ConsoleConfiguration configuration)
  {
    this.configuration = configuration;

    Dispatcher = new ImmediateDispatcher();
  }

  public event Action<int, int>? Resized;

  public int  Width     => Console.BufferWidth;
  public int  Height    => Console.BufferHeight;
  public bool IsVisible => true;
  public bool IsFocused => true;
  public bool IsClosing => false;

  public IServiceModule Services   => this;
  public IDispatcher    Dispatcher { get; }

  public void Tick(DeltaTime deltaTime)
  {
  }

  public void Dispose()
  {
  }

  void IServiceModule.RegisterServices(IServiceRegistry services)
  {
    services.AddSingleton<IKeyboardDevice>(new ConsoleKeyboardDevice());
    services.AddSingleton<IMouseDevice>(new ConsoleMouseDevice());
  }

  private sealed class ConsoleKeyboardDevice : IKeyboardDevice
  {
    public event Action<Key>? KeyPressed;
    public event Action<Key>? KeyReleased;

    public bool IsKeyDown(Key key)
    {
      return false;
    }

    public bool IsKeyUp(Key key)
    {
      return false;
    }

    public bool IsKeyPressed(Key key)
    {
      return false;
    }

    public bool IsKeyReleased(Key key)
    {
      return false;
    }
  }

  private sealed class ConsoleMouseDevice : IMouseDevice
  {
    public event Action<MouseButton>? ButtonPressed;
    public event Action<MouseButton>? ButtonReleased;
    public event Action<Vector2>?     Moved;

    public Vector2 Position      { get; }
    public Vector2 DeltaPosition { get; }

    public bool IsCursorVisible { get; set; }

    public bool IsButtonDown(MouseButton button)
    {
      return false;
    }

    public bool IsButtonUp(MouseButton button)
    {
      return false;
    }

    public bool IsButtonPressed(MouseButton button)
    {
      return false;
    }

    public bool IsButtonReleased(MouseButton button)
    {
      return false;
    }
  }
}
