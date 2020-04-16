using Surreal.Timing;

namespace Surreal.Framework.Editing.Modes
{
  public sealed class EditorModeManager
  {
    public EditorMode? CurrentMode { get; private set; }

    public void ChangeTo(EditorMode mode)
    {
      if (CurrentMode != null && !CurrentMode.IsDisposed)
      {
        CurrentMode.Dispose();
      }

      if (!mode.IsInitialized)
      {
        mode.Initialize();
      }

      CurrentMode = mode;
    }

    public void Begin()                => CurrentMode?.Begin();
    public void Input(DeltaTime time)  => CurrentMode?.Input(time);
    public void Update(DeltaTime time) => CurrentMode?.Update(time);
    public void Draw(DeltaTime time)   => CurrentMode?.Draw(time);
    public void End()                  => CurrentMode?.End();

    public void Dispose()
    {
      if (CurrentMode != null && !CurrentMode.IsDisposed)
      {
        CurrentMode.Dispose();
      }
    }
  }
}