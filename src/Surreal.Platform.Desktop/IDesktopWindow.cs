using System;

namespace Surreal.Platform
{
  public interface IDesktopWindow : IDisposable
  {
    event Action<int, int> Resized;

    string Title  { get; set; }
    int    Width  { get; set; }
    int    Height { get; set; }

    bool IsVisible       { get; set; }
    bool IsFocused       { get; }
    bool IsVsyncEnabled  { get; set; }
    bool IsCursorVisible { get; set; }
    bool IsClosing       { get; }

    void Update();
    void Present();
  }
}