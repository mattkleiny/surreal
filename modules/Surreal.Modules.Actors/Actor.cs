using System;

namespace Surreal.Modules.Actors {
  public abstract class Actor {
    private readonly IActorContext context;

    protected Actor(IActorContext context) {
      this.context = context;

      Id = context.AllocateId();
    }

    public ActorId Id { get; }

    public bool IsActive    => !IsDestroyed;
    public bool IsDestroyed { get; private set; }
    public bool IsEnabled   { get; set; }

    protected internal virtual void OnAwake() {
    }

    protected internal virtual void OnEnable() {
    }

    protected internal virtual void OnUpdate() {
    }

    protected internal virtual void OnDisable() {
    }

    protected internal virtual void OnDestroy() {
    }

    public T GetComponent<T>() {
      if (!TryGetComponent(out T component)) {
        throw new Exception($"Unable to find component {typeof(T).Name}");
      }

      return component;
    }

    public bool TryGetComponent<T>(out T component) {
      return context.GetStorage<T>().TryGetComponent(Id, out component);
    }

    public T AddComponent<T>(Optional<T> prototype = default) {
      return context.GetStorage<T>().AddComponent(Id, prototype);
    }

    public bool RemoveComponent<T>() {
      return context.GetStorage<T>().RemoveComponent(Id);
    }

    public void Destroy() {
      if (!IsDestroyed) {
        context.QueueDestroy(Id);
        IsDestroyed = true;
      }
    }
  }
}