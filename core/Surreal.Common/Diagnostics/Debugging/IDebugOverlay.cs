namespace Surreal.Diagnostics.Debugging;

/// <summary>
/// A utility for rendering immediate-mode debug overlays.
/// </summary>
public interface IDebugOverlay
{
  void ShowMenuBar(Action<IDebugMenu> builder);
  void ShowWindow(string title, Action<IDebugWindow> builder);
}

/// <summary>
/// A window in the <see cref="IDebugOverlay"/>.
/// </summary>
public interface IDebugWindow
{
  void Text(string text);
  bool Button(string text);
  bool Checkbox(string text, bool value);
}

/// <summary>
/// A menu in the <see cref="IDebugOverlay"/>.
/// </summary>
public interface IDebugMenu
{
  bool MenuItem(string title, bool enabled = true);
  void MenuItem(string title, Action<IDebugMenu> builder, bool enabled = true);
}
