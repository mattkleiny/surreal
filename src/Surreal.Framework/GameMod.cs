using System;
using System.ComponentModel.Design;
using Surreal.Timing;

namespace Surreal.Framework
{
  public interface IGameContext
  {
    IServiceContainer Services { get; }
  }

  public interface IGameMod : IDisposable
  {
    void Initialize(IGameContext context);
    void Input(DeltaTime deltaTime);
    void Update(DeltaTime deltaTime);
    void Draw(DeltaTime deltaTime);
  }

  public abstract class GameMod : IGameMod
  {
    public virtual void Initialize(IGameContext context)
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