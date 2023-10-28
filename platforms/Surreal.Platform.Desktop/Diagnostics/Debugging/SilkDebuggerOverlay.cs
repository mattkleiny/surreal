using ImGuiNET;
using Silk.NET.OpenGL.Extensions.ImGui;
using Surreal.Debugging;
using Surreal.Graphics;
using Surreal.Timing;

namespace Surreal.Diagnostics.Debugging;

/// <summary>
/// An <see cref="IDebuggerOverlay"/> for IMGUI.
/// </summary>
internal sealed class SilkDebuggerOverlay(SilkWindow window) : IDisposable, IDebuggerOverlay, IDebuggerWindow, IDebuggerMenu
{
  private readonly ImGuiController _controller = new(window.OpenGL, window.InnerWindow, window.Input);

  /// <summary>
  /// Updates the GUI.
  /// </summary>
  public void Update(DeltaTime deltaTime)
  {
    _controller.Update(deltaTime);
  }

  /// <summary>
  /// Renders the GUI.
  /// </summary>
  public void Render(IGraphicsBackend backend)
  {
    using (new GraphicsDebugScope(backend, "ImGui"))
    {
      _controller.Render();
    }
  }

  public void Dispose()
  {
    _controller.Dispose();
  }

  public void ShowMenuBar(Action<IDebuggerMenu> builder)
  {
    ImGui.BeginMainMenuBar();

    builder(this);

    ImGui.EndMainMenuBar();
  }

  public void ShowWindow(string title, Action<IDebuggerWindow> builder)
  {
    ImGui.Begin(title);

    builder(this);

    ImGui.End();
  }

  void IDebuggerWindow.Text(string text)
  {
    ImGui.Text(text);
  }

  bool IDebuggerWindow.Button(string text)
  {
    return ImGui.Button(text);
  }

  bool IDebuggerWindow.Checkbox(string text, bool value)
  {
    ImGui.Checkbox(text, ref value);

    return value;
  }

  bool IDebuggerMenu.MenuItem(string title, bool enabled)
  {
    return ImGui.MenuItem(title, enabled);
  }

  void IDebuggerMenu.MenuItem(string title, Action<IDebuggerMenu> builder, bool enabled)
  {
    if (ImGui.BeginMenu(title, enabled))
    {
      builder(this);
    }

    ImGui.EndMenu();
  }
}
