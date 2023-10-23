namespace Surreal.Graphics;

/// <summary>
/// A utility for rendering immediate-mode GUIs.
/// </summary>
public interface IDebugGui
{
  /// <summary>
  /// Shows a window in the GUI.
  /// </summary>
  void ShowWindow(string title, Action<IDebugWindow> builder);
}

/// <summary>
/// A window in the <see cref="IDebugGui"/>.
/// </summary>
public interface IDebugWindow
{
  void Text(string text);
  bool Button(string text);
  bool Checkbox(string text, bool value);
}
