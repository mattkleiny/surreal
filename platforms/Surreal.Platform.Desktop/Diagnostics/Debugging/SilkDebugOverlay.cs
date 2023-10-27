using ImGuiNET;
using Silk.NET.OpenGL.Extensions.ImGui;
using Surreal.Graphics;
using Surreal.Timing;

namespace Surreal.Diagnostics.Debugging;

/// <summary>
/// An <see cref="IDebugOverlay"/> for IMGUI.
/// </summary>
internal sealed class SilkDebugOverlay(SilkWindow window) : IDisposable, IDebugOverlay, IDebugWindow, IDebugMenu
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

  public void ShowMenuBar(Action<IDebugMenu> builder)
  {
    ImGui.BeginMainMenuBar();

    builder(this);

    ImGui.EndMainMenuBar();
  }

  public void ShowWindow(string title, Action<IDebugWindow> builder)
  {
    ImGui.Begin(title);

    builder(this);

    ImGui.End();
  }

  void IDebugWindow.Text(string text)
  {
    ImGui.Text(text);
  }

  bool IDebugWindow.Button(string text)
  {
    return ImGui.Button(text);
  }

  bool IDebugWindow.Checkbox(string text, bool value)
  {
    ImGui.Checkbox(text, ref value);

    return value;
  }

  bool IDebugMenu.MenuItem(string title, bool enabled)
  {
    return ImGui.MenuItem(title, enabled);
  }

  void IDebugMenu.MenuItem(string title, Action<IDebugMenu> builder, bool enabled)
  {
    if (ImGui.BeginMenu(title, enabled))
    {
      builder(this);
    }

    ImGui.EndMenu();
  }
}
