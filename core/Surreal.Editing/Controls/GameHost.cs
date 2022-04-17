using Avalonia.Controls;

namespace Surreal.Controls;

/// <summary>A <see cref="Control"/> which renders the output of a game.</summary>
public sealed class GameHost : Control, IEditorHostControl
{
  public event Action<int, int>? Resized;

  public new int  Width     => (int) base.Width;
  public new int  Height    => (int) base.Height;
  public     bool IsClosing => false;

  protected override void OnInitialized()
  {
    base.OnInitialized();

    LayoutUpdated += OnLayoutUpdated;
  }

  private void OnLayoutUpdated(object? sender, EventArgs e)
  {
    Resized?.Invoke(Width, Height);
  }

  void IEditorHostControl.Blit()
  {
  }
}
