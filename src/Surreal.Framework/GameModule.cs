using System;
using Surreal.Timing;

namespace Surreal
{
  /// <summary>Represents a module for a <see cref="Game"/>.</summary>
  public interface IGameModule : IDisposable
  {
    void Initialize();
    void Input(DeltaTime deltaTime);
    void Update(DeltaTime deltaTime);
    void Draw(DeltaTime deltaTime);
  }

  /// <summary>Base class for any <see cref="IGameModule"/> implementation.</summary>
  public abstract class GameModule : IGameModule
  {
    public virtual void Initialize()
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

    public virtual void Dispose()
    {
    }
  }
}
