using System;
using Surreal.Framework.Events;
using Surreal.Framework.Scenes;
using Surreal.Timing;

namespace Surreal.Framework.Simulations
{
  public class SceneSimulation<TScene> : Simulation
    where TScene : IScene
  {
    public TScene   Scene  { get; }
    public EventBus Events { get; } = new EventBus();

    public SceneSimulation(TScene scene)
    {
      Scene = scene;
    }

    public override void Initialize()
    {
      Events.RegisterListeners(this);
    }

    public override void Begin()
    {
      Scene.Begin();
    }

    public override void Input(DeltaTime deltaTime)
    {
      Scene.Input(deltaTime);
    }

    public override void Update(DeltaTime deltaTime)
    {
      Scene.Update(deltaTime);
      Events.Dispatch();
    }

    public override void Draw(DeltaTime deltaTime)
    {
      Scene.Draw(deltaTime);
    }

    public override void End()
    {
      Scene.End();
    }

    public override void Dispose()
    {
      if (Scene is IDisposable disposable)
      {
        disposable.Dispose();
      }
    }
  }
}
