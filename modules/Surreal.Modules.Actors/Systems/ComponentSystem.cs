using Surreal.Timing;

namespace Surreal.Systems;

/// <summary>Base class for any <see cref="IComponentSystem"/> implementation.</summary>
public abstract class ComponentSystem : IComponentSystem
{
  protected ComponentSystem(IComponentSystemContext context)
  {
    Context = context;
  }

  public IComponentSystemContext Context { get; }

  public virtual void OnInput(DeltaTime time)
  {
  }

  public virtual void OnUpdate(DeltaTime time)
  {
  }

  public virtual void OnDraw(DeltaTime time)
  {
  }
}
