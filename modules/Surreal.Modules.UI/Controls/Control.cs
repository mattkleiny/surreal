using Surreal.Timing;

namespace Surreal.UI.Controls;

public abstract class Control : IDisposable
{
  public bool IsLayoutDirty   { get; private set; }
  public bool IsVisuallyDirty { get; private set; }
  public bool IsDisposed      { get; private set; }

  public void MarkDirty()
  {
    IsLayoutDirty   = true;
    IsVisuallyDirty = true;
  }

  protected internal virtual void OnMeasure()
  {
  }

  protected internal virtual void OnLayout()
  {
  }

  protected internal virtual void OnUpdate(TimeDelta deltaTime)
  {
  }

  protected internal virtual void OnDraw(TimeDelta deltaTime)
  {
  }

  public void Dispose()
  {
    if (!IsDisposed)
    {
      Dispose(true);

      IsDisposed = true;
    }
  }

  protected virtual void Dispose(bool managed)
  {
  }
}