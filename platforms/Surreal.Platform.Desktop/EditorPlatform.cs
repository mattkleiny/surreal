namespace Surreal;

/// <summary>An adapter that allows <see cref="EditorPlatform"/>s to work in various contexts.</summary>
public interface IEditorHostControl
{
  event Action<int, int>? Resized;

  int  Width     { get; }
  int  Height    { get; }
  bool IsVisible { get; }
  bool IsFocused { get; }
  bool IsClosing { get; }

  // TODO: add an adapter image type?
  void Blit();
}

/// <summary>A <see cref="IPlatform"/> that supports desktop-based editors.</summary>
public sealed class EditorPlatform : IPlatform
{
  private readonly IEditorHostControl hostControl;

  public EditorPlatform(IEditorHostControl hostControl)
  {
    this.hostControl = hostControl;
  }

  public IPlatformHost BuildHost()
  {
    return new EditorPlatformHost(hostControl);
  }
}
