using Surreal.Timing;

namespace Surreal.Systems;

/// <summary>Represents a component system, capable of operating on components.</summary>
public interface ISceneSystem
{
  void OnBeginFrame(TimeDelta deltaTime);
  void OnInput(TimeDelta deltaTime);
  void OnUpdate(TimeDelta deltaTime);
  void OnDraw(TimeDelta deltaTime);
  void OnEndFrame(TimeDelta deltaTime);
}

/// <summary>Base class for any <see cref="ISceneSystem"/> implementation.</summary>
public abstract class SceneSystem : ISceneSystem
{
  public virtual void OnBeginFrame(TimeDelta deltaTime)
  {
  }

  public virtual void OnInput(TimeDelta deltaTime)
  {
  }

  public virtual void OnUpdate(TimeDelta deltaTime)
  {
  }

  public virtual void OnDraw(TimeDelta deltaTime)
  {
  }

  public virtual void OnEndFrame(TimeDelta deltaTime)
  {
  }
}
