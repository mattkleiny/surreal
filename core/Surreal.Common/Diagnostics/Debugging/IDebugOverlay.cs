namespace Surreal.Diagnostics.Debugging;

/// <summary>
/// A utility for rendering immediate-mode debug overlays.
/// </summary>
public interface IDebugOverlay
{
  /// <summary>
  /// Shows a window in the GUI.
  /// </summary>
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
