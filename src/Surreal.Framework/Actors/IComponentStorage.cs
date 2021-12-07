namespace Surreal.Actors;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class ComponentAttribute : Attribute
{
  public Type StorageType { get; }

  public ComponentAttribute(Type storageType)
  {
    StorageType = storageType;
  }

  public IComponentStorage<T> CreateStorage<T>()
  {
    var concreteType = StorageType.MakeGenericType(typeof(T));

    return (IComponentStorage<T>) Activator.CreateInstance(concreteType)!;
  }
}

public interface IComponentStorage
{
  void Prune(ActorId id);
}

public interface IComponentStorage<T> : IComponentStorage
{
  ref T GetComponent(ActorId id);
  ref T AddComponent(ActorId id, Optional<T> prototype);
  bool  RemoveComponent(ActorId id);
}
