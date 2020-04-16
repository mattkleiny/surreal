using System;
using Surreal.Timing;

namespace Surreal
{
  public interface IMod : IDisposable
  {
    void Initialize(IModRegistry registry);
    void Begin();
    void Input(DeltaTime deltaTime);
    void Update(DeltaTime deltaTime);
    void Draw(DeltaTime deltaTime);
    void End();
  }

  public abstract class Mod : IMod
  {
    public virtual void Initialize(IModRegistry registry)
    {
    }

    public virtual void Begin()
    {
    }

    public virtual void Input(DeltaTime deltaTime)
    {
    }

    public virtual void Update(DeltaTime deltaTime)
    {
    }

    public virtual void Draw(DeltaTime deltaTime)
    {
    }

    public virtual void End()
    {
    }

    public virtual void Dispose()
    {
    }
  }
}
