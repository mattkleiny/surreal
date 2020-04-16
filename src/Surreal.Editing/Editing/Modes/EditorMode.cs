using System;
using Surreal.Timing;

namespace Surreal.Framework.Editing.Modes
{
  public abstract class EditorMode : IDisposable
  {
    public bool IsInitialized { get; private set; }
    public bool IsDisposed    { get; private set; }

    public virtual void Initialize()
    {
      IsInitialized = true;
    }

    public virtual void Begin()
    {
    }

    public virtual void Input(DeltaTime time)
    {
    }

    public virtual void Update(DeltaTime time)
    {
    }

    public virtual void Draw(DeltaTime time)
    {
    }

    public virtual void End()
    {
    }

    public virtual void Dispose()
    {
      IsDisposed = true;
    }
  }
}