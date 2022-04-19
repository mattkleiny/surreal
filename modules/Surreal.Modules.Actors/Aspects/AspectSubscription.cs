using Surreal.Components;

namespace Surreal.Aspects;

/// <summary>A subscription to changes in an <see cref="ActorScene"/> that correspond with a particular <see cref="Aspect"/>.</summary>
public sealed class AspectSubscription : IDisposable
{
  public event Action<ActorId>? Added;
  public event Action<ActorId>? Removed;

  internal AspectSubscription(Aspect aspect, IActorContext context)
  {
    Aspect  = aspect;
    Context = context;
  }

  public   Aspect        Aspect  { get; }
  internal IActorContext Context { get; }

  internal void NotifyChanged(ActorId id, ComponentMask added, ComponentMask removed)
  {
    // TODO: notify changes on this actor
    if (Aspect.IsInterestedIn(added))
    {
      Added?.Invoke(id);
    }
  }

  public void Dispose()
  {
    Context.RemoveSubscription(this);
  }
}
