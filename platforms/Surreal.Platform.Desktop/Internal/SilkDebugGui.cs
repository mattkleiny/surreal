using ImGuiNET;
using Silk.NET.OpenGL.Extensions.ImGui;
using Surreal.Graphics;
using Surreal.Timing;

namespace Surreal;

/// <summary>
/// An <see cref="IDebugGui"/> for IMGUI.
/// </summary>
internal sealed class SilkDebugGui(SilkWindow window) : IDebugGui, IDisposable, IDebugWindow
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

  public void ShowWindow(string title, Action<IDebugWindow> builder)
  {
    ImGui.Begin(title);

    builder(this);

    ImGui.End();
  }

  public void Dispose()
  {
    _controller.Dispose();
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
}
