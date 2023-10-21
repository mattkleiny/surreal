using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Surreal.Graphics.Images;

namespace Surreal;

internal sealed class SilkWindow : IDesktopWindow
{
  private readonly IWindow _window;
  private bool _isFocused;

  public SilkWindow(DesktopConfiguration configuration)
  {
    var windowOptions = WindowOptions.Default with
    {
      Title = configuration.Title,
      Size = new Vector2D<int>(configuration.Width, configuration.Height),
      IsEventDriven = configuration.IsEventDriven,
      VSync = configuration.IsVsyncEnabled,
      WindowBorder = configuration.IsResizable ? WindowBorder.Resizable : WindowBorder.Fixed,
    };

    _window = Window.Create(windowOptions);

    _window.FocusChanged += OnFocusChanged;
    _window.Resize += OnWindowResized;

    _window.Initialize();

    OpenGL = _window.CreateOpenGL();
    Input = _window.CreateInput();
  }

  public GL OpenGL { get; }
  public IInputContext Input { get; }

  public event Action<int, int>? Resized;

  public string Title
  {
    get => _window.Title;
    set => _window.Title = value;
  }

  public int Width
  {
    get => _window.Size.X;
    set => _window.Size = _window.Size with { X = value };
  }

  public int Height
  {
    get => _window.Size.Y;
    set => _window.Size = _window.Size with { Y = value };
  }

  public bool IsVisible
  {
    get => _window.IsVisible;
    set => _window.IsVisible = value;
  }

  public bool IsVsyncEnabled
  {
    get => _window.VSync;
    set => _window.VSync = value;
  }

  public bool IsEventDriven
  {
    get => _window.IsEventDriven;
    set => _window.IsEventDriven = value;
  }

  public bool IsFocused => _isFocused;
  public bool IsClosing => _window.IsClosing;

  private void OnFocusChanged(bool isFocused)
  {
    _isFocused = isFocused;
  }

  private void OnWindowResized(Vector2D<int> size)
  {
    Resized?.Invoke(size.X, size.Y);
  }

  public void SetWindowIcon(Image image)
  {
    throw new NotImplementedException();
  }

  public void Update()
  {
    _window.DoEvents();

    if (!_window.IsClosing)
    {
      _window.DoUpdate();
    }
  }

  public void Present()
  {
    if (!_window.IsClosing)
    {
      _window.DoRender();
    }
  }

  public void Dispose()
  {
    Input.Dispose();

    _window.Reset();
    _window.Dispose();
  }
}
