﻿using Surreal.Timing;

namespace Surreal.Systems;

/// <summary>Represents a component system, capable of operating on components.</summary>
public interface ISceneSystem
{
  public IActorContext? Context { get; set; }

  void OnBeginFrame(DeltaTime deltaTime);
  void OnInput(DeltaTime deltaTime);
  void OnUpdate(DeltaTime deltaTime);
  void OnDraw(DeltaTime deltaTime);
  void OnEndFrame(DeltaTime deltaTime);
}

/// <summary>Base class for any <see cref="ISceneSystem"/> implementation.</summary>
public abstract class SceneSystem : ISceneSystem
{
  public IActorContext? Context { get; set; }

  protected ref T GetComponent<T>(ActorId id)
    where T : notnull, new()
  {
    if (Context == null)
    {
      throw new InvalidOperationException("This system is not attached to a context");
    }

    return ref Context.GetStorage<T>().GetComponent(id);
  }

  public virtual void OnBeginFrame(DeltaTime deltaTime)
  {
  }

  public virtual void OnInput(DeltaTime deltaTime)
  {
  }

  public virtual void OnUpdate(DeltaTime deltaTime)
  {
  }

  public virtual void OnDraw(DeltaTime deltaTime)
  {
  }

  public virtual void OnEndFrame(DeltaTime deltaTime)
  {
  }
}
