namespace Surreal.Debugging;

/// <summary>
/// A utility for rendering immediate-mode debug overlays.
/// </summary>
public interface IDebuggerOverlay
{
  void ShowMenuBar(Action<IDebuggerMenu> builder);
  void ShowWindow(string title, Action<IDebuggerWindow> builder);
}

/// <summary>
/// A window in the <see cref="IDebuggerOverlay"/>.
/// </summary>
public interface IDebuggerWindow
{
  void Text(string text);
  bool Button(string text);
  bool Checkbox(string text, bool value);
}

/// <summary>
/// A menu in the <see cref="IDebuggerOverlay"/>.
/// </summary>
public interface IDebuggerMenu
{
  bool MenuItem(string title, bool enabled = true);
  void MenuItem(string title, Action<IDebuggerMenu> builder, bool enabled = true);
}
