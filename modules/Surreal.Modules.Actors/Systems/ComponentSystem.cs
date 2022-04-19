using Surreal.Aspects;

namespace Surreal.Systems;

/// <summary>Base class for any <see cref="ISceneSystem"/> implementation that monitors components..</summary>
public abstract class ComponentSystem : SceneSystem
{
  private readonly Aspect aspect;

  protected ComponentSystem(Aspect aspect)
  {
    this.aspect = aspect;
  }
}
